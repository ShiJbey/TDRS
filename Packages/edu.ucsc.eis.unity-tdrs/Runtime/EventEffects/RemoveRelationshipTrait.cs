using UnityEngine;

namespace TDRS
{
	public class RemoveRelationshipTraitFactory : SocialEventEffectFactory
	{
		public class RemoveRelationshipTrait : ISocialEventEffect
		{
			protected SocialRelationship m_relationship;
			protected string m_traitID;

			public RemoveRelationshipTrait(
				SocialRelationship relationship,
				string traitID
			)
			{
				m_relationship = relationship;
				m_traitID = traitID;
			}

			public void Apply()
			{
				m_relationship.RemoveTrait(m_traitID);
			}

			public void Remove()
			{
				// trait removal is permanent
				return;
			}
		}

		public override string EffectName => "RemoveRelationshipTrait";

		public override ISocialEventEffect CreateInstance(EffectBindingContext ctx, params string[] args)
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
				ctx.Engine.GetRelationship(
					ctx.Bindings[relationshipOwnerVar],
					ctx.Bindings[relationshipTargetVar]
				),
				traitID
			);
		}
	}
}
