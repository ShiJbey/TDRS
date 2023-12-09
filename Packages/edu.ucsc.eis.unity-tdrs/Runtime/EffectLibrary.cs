using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// A repository of effect factories
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
