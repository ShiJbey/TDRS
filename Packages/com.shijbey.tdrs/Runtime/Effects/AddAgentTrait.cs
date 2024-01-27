namespace TDRS
{
	public class AddAgentTrait : IEffect
	{
		protected AgentNode m_agent;
		protected string m_traitID;
		protected int m_duration;

		public AddAgentTrait(
			AgentNode agent,
			string traitID,
			int duration
		)
		{
			m_agent = agent;
			m_traitID = traitID;
			m_duration = duration;
		}

		public void Apply()
		{
			m_agent.AddTrait(m_traitID, m_duration);
		}

		public void Remove()
		{
			// Trait additions are permanent
			return;
		}

		public override string ToString()
		{
			return $"AddAgentTrait {m_agent.UID} {m_traitID} {(m_duration == -1 ? "" : m_duration)}";
		}
	}
}
