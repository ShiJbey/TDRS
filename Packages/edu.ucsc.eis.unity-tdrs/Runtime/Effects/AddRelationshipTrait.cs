using UnityEngine;

namespace TDRS
{
	public class AddRelationshipTraitFactory : EffectFactory
	{
		public class AddRelationshipTrait : IEffect
		{
			protected SocialRelationship m_relationship;
			protected string m_traitID;
			protected int m_duration;

			public AddRelationshipTrait(
				SocialRelationship relationship,
				string traitID,
				int duration
			)
			{
				m_relationship = relationship;
				m_traitID = traitID;
				m_duration = duration;
			}

			public void Apply()
			{
				m_relationship.AddTrait(m_traitID, m_duration);
			}

			public void Remove()
			{
				// Trait additions are permanent
				return;
			}
		}

		public override string EffectName => "AddRelationshipTrait";

		public override IEffect CreateInstance(EffectBindingContext ctx, params string[] args)
		{
			if (args.Length < 3)
			{
				string argStr = string.Join(" ", args);

				throw new System.ArgumentException(
					$"Incorrect number of arguments for 'AddRelationshipTrait {argStr}'. "
					+ $"Expected at least 3 but was {args.Length}."
				);
			}

			string relationshipOwnerVar = args[0];
			string relationshipTargetVar = args[1];
			string traitID = args[2];
			int duration = -1;

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

			if (args.Length >= 4)
			{
				if (!int.TryParse(args[3], out var value))
				{
					throw new System.ArgumentException(
						$"Expected integer as 4th argument but was '{args[3]}'"
					);
				}
				duration = value;
			}

			return new AddRelationshipTrait(
				ctx.Engine.GetRelationship(
					ctx.Bindings[relationshipOwnerVar],
					ctx.Bindings[relationshipTargetVar]
				),
				traitID,
				duration
			);
		}
	}
}