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

		#region Properties

		public IList<IEffect> Effects => m_effects;

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
		}

		public bool RemoveEffect(IEffect effect)
		{
			return m_effects.Remove(effect);
		}

		public void TickEffects()
		{
			List<IEffect> effectList = new List<IEffect>(Effects);

			// Loop backward incase an effect needs to be removed.
			foreach (Effect effect in effectList)
			{
				effect.Tick();

				if (!effect.IsValid)
				{
					effect.Remove();
					effect.IsActive = false;
					RemoveEffect(effect);
				}
			}
		}

		#endregion
	}
}
