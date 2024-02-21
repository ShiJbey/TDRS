namespace TDRS
{
	public class RemoveRelationshipTrait : IEffect
	{
		#region Fields

		private Relationship m_relationship;
		private string m_traitID;

		#endregion

		#region Constructors

		public RemoveRelationshipTrait(
			Relationship relationship,
			string traitID
		)
		{
			m_relationship = relationship;
			m_traitID = traitID;
		}

		#endregion

		#region Public Methods

		public void Apply()
		{
			m_relationship.RemoveTrait(m_traitID);
		}

		public override string ToString()
		{
			return $"Remove {m_traitID} relationship trait";
		}

		#endregion
	}
}
