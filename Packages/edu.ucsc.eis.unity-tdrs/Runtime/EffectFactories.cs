using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// Manages references to effect factories used to create social events and traits
	/// </summary>
	public class EffectFactories : MonoBehaviour
	{
		#region Fields

		/// <summary>
		/// Lookup table of factories by effect name
		/// </summary>
		protected Dictionary<string, ISocialEventEffectFactory> m_factoryLookup;

		#endregion

		#region Unity Messages

		private void Awake()
		{
			m_factoryLookup = new Dictionary<string, ISocialEventEffectFactory>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a factory to the library.
		/// </summary>
		/// <param name="effectName"></param>
		/// <param name="factory"></param>
		public void AddEffectFactory(string effectName, ISocialEventEffectFactory factory)
		{
			m_factoryLookup[effectName] = factory;
		}

		/// <summary>
		/// Get an effect factory by event type
		/// </summary>
		/// <param name="effectName"></param>
		/// <returns></returns>
		public ISocialEventEffectFactory GetEffectFactory(string effectName)
		{
			return m_factoryLookup[effectName];
		}

		/// <summary>
		/// Add the various factory instances on the same gameobject
		/// </summary>
		public void RegisterFactories()
		{
			var effectFactories = GetComponents<SocialEventEffectFactory>();
			foreach (var factory in effectFactories)
			{
				AddEffectFactory(factory.EffectName, factory);
			}
		}

		/// <summary>
		/// Instantiate a new event instance
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="eventString"></param>
		/// <returns></returns>
		public ISocialEventEffect CreateInstance(EffectBindingContext ctx, string effectSting)
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
