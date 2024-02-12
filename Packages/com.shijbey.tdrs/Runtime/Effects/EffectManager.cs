using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// Manages all the effects currently applied to the agent or relationship.
	/// </summary>
	public class EffectManager
	{
		#region Fields

		protected List<IEffect> m_effects;

		#endregion

		#region Constructors

		public EffectManager()
		{
			m_effects = new List<IEffect>();
		}

		#endregion

		#region Public Methods

		public void AddEffect(IEffect effect)
		{
			m_effects.Add(effect);
			effect.Apply();
		}

		public bool RemoveEffect(IEffect effect)
		{
			if (m_effects.Remove(effect))
			{
				effect.Remove();
				return true;
			}

			return false;
		}

		public bool RemoveAllFromSource(object source)
		{
			var removed_any_effect = false;

			for (int i = m_effects.Count - 1; i >= 0; i--)
			{
				if (m_effects[i].Source == source)
				{
					m_effects[i].Remove();
					m_effects.RemoveAt(i);
					removed_any_effect = true;
				}
			}

			return removed_any_effect;
		}

		public void TickEffects()
		{
			for (int i = m_effects.Count - 1; i >= 0; i--)
			{
				var effect = m_effects[i];
				effect.Tick();

				if (!effect.IsValid)
				{
					effect.Remove();
					m_effects.RemoveAt(i);
				}
			}
		}

		#endregion
	}
}
