using System.Linq;
using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// Manages references to effect factories used to create social events and traits
	/// </summary>
	public class EffectLibrary
	{
		#region Fields

		/// <summary>
		/// Lookup table of factories by effect name
		/// </summary>
		protected Dictionary<string, IEffectFactory> m_factories;

		#endregion

		#region Constructors

		public EffectLibrary()
		{
			m_factories = new Dictionary<string, IEffectFactory>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a factory to the library.
		/// </summary>
		/// <param name="effectName"></param>
		/// <param name="factory"></param>
		public void AddEffectFactory(IEffectFactory factory)
		{
			m_factories[factory.EffectName] = factory;
		}

		/// <summary>
		/// Get an effect factory by event type
		/// </summary>
		/// <param name="effectName"></param>
		/// <returns></returns>
		public IEffectFactory GetEffectFactory(string effectName)
		{
			return m_factories[effectName];
		}

		/// <summary>
		/// Instantiate a new event instance
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="eventString"></param>
		/// <returns></returns>
		public IEffect CreateInstance(EffectBindingContext ctx, string effectSting)
		{
			List<string> effectParts = effectSting
					.Split(" ").Select(s => s.Trim()).ToList();

			string effectName = effectParts[0]; // The effect name is the first part
			effectParts.RemoveAt(0); // Remove the name from the front of the list

			// Get the factory
			var effectFactory = GetEffectFactory(effectName);

			var effect = effectFactory.CreateInstance(ctx, effectParts.ToArray());

			return effect;
		}

		#endregion
	}
}
