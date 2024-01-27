namespace TDRS
{
	public class AddRelationshipTrait : IEffect
	{
		protected RelationshipEdge m_relationship;
		protected string m_traitID;
		protected int m_duration;

		public AddRelationshipTrait(
			RelationshipEdge relationship,
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

		public override string ToString()
		{
			return $"AddRelationshipTrait {m_relationship.Owner.UID} {m_relationship.Target.UID} {m_traitID} {(m_duration == -1 ? "" : m_duration)}";
		}
	}
}
