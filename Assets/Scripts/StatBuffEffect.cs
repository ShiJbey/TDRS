namespace TDRS.Sample
{
	public class StatBuffEffect : IEffect
	{
		protected string _statName;
		protected float _amount;

		public StatBuffEffect(string statName, float amount)
		{
			_statName = statName;
			_amount = amount;
		}

		public string Description => $"Add {_amount} modifier to {_statName}";

		public void Apply(SocialEntity target)
		{
			if (!target.Stats.ContainsKey(_statName))
			{
				throw new System.Exception($"Cannot find {_statName} stat for {target.EntityID}");
			}

			target.Stats[_statName].AddModifier(
				new StatSystem.StatModifier(
					_amount,
					StatSystem.StatModifierType.FLAT,
					this
				)
			);
		}

		public void Remove(SocialEntity target)
		{
			if (!target.Stats.ContainsKey(_statName))
			{
				throw new System.Exception($"Cannot find {_statName} stat for {target.EntityID}");
			}

			target.Stats[_statName].RemoveModifiersFromSource(this);
		}
	}
}
