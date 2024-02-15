namespace TDRS
{
	public class IncreaseRelationshipStat : Effect
	{
		#region Fields

		protected Relationship m_relationship;
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

		public IncreaseRelationshipStat(
			EffectContext ctx,
			Relationship relationship,
			string statName,
			float value,
			int duration
		) : base(relationship, ctx, duration)
		{
			m_relationship = relationship;
			m_statName = statName;
			m_value = value;
		}

		#endregion

		#region Public Method

		public override void Apply()
		{
			m_relationship.Stats.GetStat(m_statName).AddModifier(
				new StatModifier(
					m_value,
					StatModifierType.FLAT,
					this
				)
			);
			m_relationship.Effects.AddEffect(this);
		}

		public override void Remove()
		{
			m_relationship.Stats.GetStat(m_statName).RemoveModifiersFromSource(this);
			m_relationship.Effects.RemoveEffect(this);
		}

		#endregion
	}
}
