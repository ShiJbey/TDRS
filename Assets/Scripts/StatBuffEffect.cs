using UnityEngine;

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
			return;
		}

		public void Remove(SocialEntity target)
		{
			return;
		}
	}
}
