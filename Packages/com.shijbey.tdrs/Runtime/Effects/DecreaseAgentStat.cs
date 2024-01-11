namespace TDRS
{
	public class DecreaseAgentStat : IEffect
	{
		protected SocialAgent m_agent;
		protected string m_statName;
		protected float m_value;
		protected int m_duration;
		protected string m_reason;

		public DecreaseAgentStat(
			SocialAgent agent,
			string statName,
			float value,
			int duration,
			string reason
		)
		{
			m_agent = agent;
			m_statName = statName;
			m_value = value;
			m_duration = duration;
			m_reason = reason;
		}

		public void Apply()
		{
			m_agent.Stats.AddModifier(
				new StatSystem.StatModifier(
					m_statName,
					m_reason,
					-m_value,
					StatSystem.StatModifierType.FLAT,
					m_duration,
					this
				)
			);
		}

		public void Remove()
		{
			m_agent.Stats.RemoveModifiersFromSource(this);
		}

		public override string ToString()
		{
			return $"DecreaseAgentStat {m_agent.UID} {m_statName} {m_value} {(m_duration == -1 ? "" : m_duration)}";
		}
	}
}
