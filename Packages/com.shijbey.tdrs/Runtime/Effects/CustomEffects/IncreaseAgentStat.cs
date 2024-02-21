namespace TDRS
{
	public class IncreaseAgentStat : IEffect
	{
		#region Fields

		private Agent m_agent;
		private string m_statName;
		private float m_value;

		#endregion

		#region Constructors

		public IncreaseAgentStat(
			Agent agent,
			string statName,
			float value
		)
		{
			m_agent = agent;
			m_statName = statName;
			m_value = value;
		}

		#endregion

		#region Public Methods

		public void Apply()
		{
			m_agent.Stats.GetStat(m_statName).BaseValue += m_value;
		}

		public override string ToString()
		{
			string sign = (m_value >= 0) ? "+" : "-";
			return $"{sign}{m_value} {m_statName} stat";
		}

		#endregion
	}
}
