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
		}

		public override string EffectType => "RemoveRelationshipTrait";

		public override ISocialEventEffect CreateInstance(SocialEventContext ctx, params string[] args)
		{
			if (args.Length != 3)
			{
				throw new System.ArgumentException(
					"Incorrect number of arguments for RemoveRelationshipTrait. "
					+ $"Expected 3 but was {args.Length}."
				);
			}

			string relationshipOwnerVar = args[0];
			string relationshipTargetVar = args[1];
			string traitID = args[2];

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
