namespace TDRS
{
	public class DecreaseRelationshipStat : Effect
	{
		#region Fields

		private Relationship m_relationship;
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

		public override bool IsValid
		{
			get
			{
				return !HasDuration || (HasDuration && RemainingDuration > 0);
			}
		}

		#endregion

		#region Constructors

		public DecreaseRelationshipStat(
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

		#region Public Methods

		public override void Apply()
		{
			m_relationship.Stats.GetStat(m_statName).AddModifier(
				new StatModifier(
					-m_value,
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
