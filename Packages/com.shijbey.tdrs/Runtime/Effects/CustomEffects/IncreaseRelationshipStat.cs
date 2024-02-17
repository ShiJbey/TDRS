namespace TDRS
{
	public class IncreaseRelationshipStat : IEffect
	{
		#region Fields

		private Relationship m_relationship;
		private string m_statName;
		private float m_value;

		#endregion

		#region Constructors

		public IncreaseRelationshipStat(
			Relationship relationship,
			string statName,
			float value
		)
		{
			m_relationship = relationship;
			m_statName = statName;
			m_value = value;
		}

		#endregion

		#region Public Method

		public void Apply()
		{
			m_relationship.Stats.GetStat(m_statName).BaseValue += m_value;
		}

		public override string ToString()
		{
			string sign = (m_value >= 0) ? "+" : "-";
			return $"{sign}{m_value} {m_statName} stat";
		}

		#endregion
	}
}
