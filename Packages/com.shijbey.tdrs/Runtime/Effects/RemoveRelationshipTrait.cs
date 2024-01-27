namespace TDRS
{
	public class RemoveRelationshipTrait : IEffect
	{
		protected RelationshipEdge m_relationship;
		protected string m_traitID;

		public RemoveRelationshipTrait(
			RelationshipEdge relationship,
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

		public override string ToString()
		{
			return $"RemoveRelationshipTrait {m_relationship.Owner.UID} {m_relationship.Target.UID} {m_traitID}";
		}
	}
}
