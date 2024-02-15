namespace TDRS
{
	public class IncreaseAgentStat : Effect
	{
		#region Fields

		protected Agent m_agent;
		protected string m_statName;
		protected float m_value;

		#endregion

		#region Properties

		public override string Description
		{
			get
			{
				string sign = (m_value >= 0) ? "+" : "-";
				return $"{sign}{m_value} {m_statName} stat";
			}
		}

		#endregion

		#region Constructors

		public IncreaseAgentStat(
			EffectContext ctx,
			Agent agent,
			string statName,
			float value,
			int duration
		) : base(agent, ctx, duration)
		{
			m_agent = agent;
			m_statName = statName;
			m_value = value;
		}

		#endregion

		#region Public Methods

		public override void Apply()
		{
			m_agent.Stats.GetStat(m_statName).AddModifier(
				new StatModifier(
					m_value,
					StatModifierType.FLAT,
					this
				)
			);
			m_agent.Effects.AddEffect(this);
		}

		public override void Remove()
		{
			m_agent.Stats.GetStat(m_statName).RemoveModifiersFromSource(this);
			m_agent.Effects.RemoveEffect(this);
		}

		#endregion
	}
}
