namespace TDRS
{
	public class DecreaseAgentStat : Effect
	{
		#region Fields

		private Agent m_agent;
		private string m_statName;
		private float m_value;

		#endregion

		#region Properties

		public override string Description
		{
			get
			{
				// Sign is opposite because a positive effect value means we are decreasing
				// the stat by that amount. A negative decrease is an increase.
				string sign = (m_value >= 0) ? "-" : "+";
				return $"{sign}{m_value} {m_statName} stat";
			}
		}

		#endregion

		#region Constructors

		public DecreaseAgentStat(
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
					-m_value,
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
