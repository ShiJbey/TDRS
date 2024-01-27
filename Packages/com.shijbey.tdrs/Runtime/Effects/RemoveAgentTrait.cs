namespace TDRS
{
	public class RemoveAgentTrait : IEffect
	{
		protected AgentNode m_agent;
		protected string m_traitID;

		public RemoveAgentTrait(
			AgentNode agent,
			string traitID
		)
		{
			m_agent = agent;
			m_traitID = traitID;
		}

		public void Apply()
		{
			m_agent.RemoveTrait(m_traitID);
		}

		public void Remove()
		{
			// Trait removal is permanent
			return;
		}

		public override string ToString()
		{
			return $"RemoveAgentTrait {m_agent.UID} {m_traitID}";
		}
	}
}
