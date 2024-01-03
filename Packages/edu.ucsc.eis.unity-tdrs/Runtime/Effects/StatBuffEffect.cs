namespace TDRS
{
	public class StatBuffEffect : IEffect
	{
		/// <summary>
		/// The name of the stat to modify
		/// </summary>
		protected string _statName;

		/// <summary>
		/// The amount to modify the stat by
		/// </summary>
		protected float _amount;

		/// <summary>
		/// The reason for this effect
		/// </summary>
		protected string _reason;

		public StatBuffEffect(string statName, float amount, string reason)
		{
			_statName = statName;
			_amount = amount;
			_reason = reason;
		}

		public string Description
		{
			get
			{
				string amountPrefix = _amount > 0 ? "+" : "";
				return $"{_statName} {amountPrefix}{_amount}: {_reason}";
			}

		}

		public void Apply(SocialEntity target)
		{
			target.Stats.AddModifier(
				new StatSystem.StatModifier(
					_statName,
					Description,
					_amount,
					StatSystem.StatModifierType.FLAT,
					this
				)
			);
		}

		public void Remove(SocialEntity target)
		{
			target.Stats.RemoveModifiersFromSource(this);
		}
	}
}
