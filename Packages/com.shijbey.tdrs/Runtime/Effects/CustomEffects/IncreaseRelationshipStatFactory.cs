namespace TDRS
{
	/// <summary>
	/// Constructs IncreaseRelationshipStat effect instances
	/// </summary>
	public class IncreaseRelationshipStatFactory : IEffectFactory
	{
		public string EffectName => "IncreaseRelationshipStat";

		public IEffect CreateInstance(EffectBindingContext ctx, params string[] args)
		{
			if (args.Length < 4)
			{
				string argStr = string.Join(" ", args);

				throw new System.ArgumentException(
					$"Incorrect number of arguments for 'IncreaseRelationshipStat {argStr}'. "
					+ $"Expected 4 but was {args.Length}."
				);
			}

			string relationshipOwnerVar = args[0];
			string relationshipTargetVar = args[1];
			string statName = args[2];

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

			if (!float.TryParse(args[3], out var value))
			{
				throw new System.ArgumentException(
					$"Expected number as last argument but was '{args[3]}'"
				);
			}

			int duration = -1;

			if (args.Length >= 5)
			{
				if (!int.TryParse(args[4], out duration))
				{
					throw new System.ArgumentException(
						$"Expected integer as 5th argument but was '{args[4]}'"
					);
				}
			}

			return new IncreaseRelationshipStat(
				ctx,
				ctx.Engine.GetRelationship(
					ctx.Bindings[relationshipOwnerVar],
					ctx.Bindings[relationshipTargetVar]
				),
				statName,
				value,
				duration
			);
		}
	}
}
