namespace TDRS
{
	/// <summary>
	/// Constructs RemoveRelationshipTrait effect instances
	/// </summary>
	public class RemoveRelationshipTraitFactory : IEffectFactory
	{
		public string EffectName => "RemoveRelationshipTrait";

		public IEffect CreateInstance(EffectBindingContext ctx, params string[] args)
		{
			if (args.Length != 3)
			{
				string argStr = string.Join(" ", args);

				throw new System.ArgumentException(
					$"Incorrect number of arguments for 'RemoveRelationshipTrait {argStr}'. "
					+ $"Expected 3 but was {args.Length}."
				);
			}

			string relationshipOwnerVar = args[0];
			string relationshipTargetVar = args[1];
			string traitID = args[2];

			if (!ctx.Engine.HasRelationship(
					ctx.Bindings[relationshipOwnerVar],
					ctx.Bindings[relationshipTargetVar]
					)
				)
			{
				throw new System.ArgumentException(
					"No relationship found from "
					+ $"{ctx.Bindings[relationshipOwnerVar]} to"
					+ $"{ctx.Bindings[relationshipTargetVar]}."
				);
			}

			return new RemoveRelationshipTrait(
				ctx,
				ctx.Engine.GetRelationship(
					ctx.Bindings[relationshipOwnerVar],
					ctx.Bindings[relationshipTargetVar]
				),
				traitID
			);
		}
	}
}
