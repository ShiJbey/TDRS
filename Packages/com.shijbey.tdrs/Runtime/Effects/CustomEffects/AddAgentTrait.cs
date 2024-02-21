namespace TDRS
{
	public class AddAgentTrait : IEffect
	{
		#region Fields

		private Agent m_agent;
		private string m_traitID;
		private int m_duration;
		private string m_descriptionOverride;

		#endregion

		#region Constructors

		public AddAgentTrait(
			Agent agent,
			string traitID,
			int duration,
			string descriptionOverride
		)
		{
			m_agent = agent;
			m_traitID = traitID;
			m_duration = duration;
			m_descriptionOverride = descriptionOverride;
		}

		#endregion

		#region Public Methods

		public void Apply()
		{
			m_agent.AddTrait(m_traitID, m_duration, m_descriptionOverride);
		}

		public override string ToString()
		{
			return $"Add {m_traitID} trait";
		}

		#endregion
	}
}
