using System.Collections.Generic;

namespace TDRS
{
    /// <summary>
    /// A repository of alll the possible effect types available in the game.
    /// </summary>
    public class EffectLibrary
    {
        #region Attributes
        /// <summary>
        /// Precondition IDs mapped to factories that construct that effect.
        /// </summary>
        protected Dictionary<string, IEffectFactory> _factories;
        #endregion

        #region Constructor
        public EffectLibrary()
        {
            _factories = new Dictionary<string, IEffectFactory>();
        }
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
        /// <param name="args"></param>
        /// <returns></returns>
        public IEffect CreateFromData(string effectID, params object?[] args)
        {
            var factory = _factories[effectID];

            var effect = factory.Instantiate(args);

            return effect;
        }
        #endregion
    }
}
