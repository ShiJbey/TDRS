using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace TDRS
{
	/// <summary>
	/// A repository of effect effect types
	/// </summary>
	public class EffectLibrary
	{
		#region Attributes

		/// <summary>
		/// Precondition IDs mapped to factories that construct that effect.
		/// </summary>
		protected Dictionary<string, IEffectFactory> _factories = new Dictionary<string, IEffectFactory>();

		/// <summary>
		/// Factories to import into the library
		/// </summary>
		public List<EffectFactorySO> effectFactories = new List<EffectFactorySO>();

		#endregion

		#region Methods

		/// <summary>
		/// Add a new factory to the library.
		/// </summary>
		/// <param name="effectID">
		/// The effect ID the factory will be used for.
		/// </param>
		/// <param name="factory">
		/// The factory instance.
		/// </param>
		public void AddFactory(string effectID, IEffectFactory factory)
		{
			_factories[effectID] = factory;
		}

		/// <summary>
		/// Create a new effect instance.
		/// </summary>
		/// <param name="effectID"></param>
		/// <param name="effectNode"></param>
		/// <returns></returns>
		public IEffect CreateEffect(TDRSManager manager, string effectID, YamlNode effectNode)
		{
			var factory = _factories[effectID];

			var effect = factory.Instantiate(manager, effectNode);

			return effect;
		}

		/// <summary>
		/// Get the effect factory mapped to a given ID.
		/// </summary>
		/// <param name="effectID"></param>
		/// <returns></returns>
		public IEffectFactory GetEffectFactory(string effectID)
		{
			return _factories[effectID];
		}

		#endregion
	}
}
