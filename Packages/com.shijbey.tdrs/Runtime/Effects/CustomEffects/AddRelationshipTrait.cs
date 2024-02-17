namespace TDRS
{
	public class AddRelationshipTrait : IEffect
	{
		#region Fields

		private Relationship m_relationship;
		private string m_traitID;
		private string m_descriptionOverride;
		private int m_duration;

		#endregion

		#region Constructors

		public AddRelationshipTrait(
			Relationship relationship,
			string traitID,
			int duration,
			string descriptionOverride
		)
		{
			m_relationship = relationship;
			m_traitID = traitID;
			m_duration = duration;
			m_descriptionOverride = descriptionOverride;
		}

		#endregion

		#region Public Methods

		public void Apply()
		{
			m_relationship.AddTrait(m_traitID, m_duration, m_descriptionOverride);
		}

		public override string ToString()
		{
			return $"Add {m_traitID} relationship trait.";
		}

		#endregion
	}
}
