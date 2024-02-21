namespace TDRS
{
	public class RemoveAgentTrait : IEffect
	{
		#region Fields

		private Agent m_agent;
		private string m_traitID;

		#endregion

		#region Constructors

		public RemoveAgentTrait(
			Agent agent,
			string traitID
		)
		{
			m_agent = agent;
			m_traitID = traitID;
		}

		#endregion

		#region Public Methods

		public void Apply()
		{
			m_agent.RemoveTrait(m_traitID);
		}

		public override string ToString()
		{
			return $"Remove {m_traitID} trait.";
		}

		#endregion
	}
}
