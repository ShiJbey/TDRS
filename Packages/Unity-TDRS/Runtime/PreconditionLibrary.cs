using System.Collections.Generic;

namespace TDRS
{
    /// <summary>
    /// A repository of alll the possible precondition types available in the game.
    /// </summary>
    public class PreconditionLibrary
    {
        #region Attributes
        /// <summary>
        /// Precondition IDs mapped to factories that construct that precondition.
        /// </summary>
        protected Dictionary<string, IPreconditionFactory> _factories;
        #endregion

        #region Constructor
        public PreconditionLibrary()
        {
            _factories = new Dictionary<string, IPreconditionFactory>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add a new factory to the library.
        /// </summary>
        /// <param name="preconditionID">
        /// The precondition ID the factory will be used for.
        /// </param>
        /// <param name="factory">
        /// The factory instance.
        /// </param>
        public void AddFactory(string preconditionID, IPreconditionFactory factory)
        {
            _factories[preconditionID] = factory;
        }

        /// <summary>
        /// Create a new precondition instance.
        /// </summary>
        /// <param name="preconditionID"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IPrecondition CreateFromData(string preconditionID, params object?[] args)
        {
            var factory = _factories[preconditionID];

            var precondition = factory.Instantiate(args);

            return precondition;
        }
        #endregion
    }
}
