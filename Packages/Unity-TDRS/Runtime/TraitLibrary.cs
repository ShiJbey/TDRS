using System.Collections.Generic;
using System.Linq;

namespace TDRS
{
    /// <summary>
    /// A repository of all the various trait types that exist in the game.
    /// </summary>
    public class TraitLibrary
    {
        #region Attributes
        /// <summary>
        /// Repository of trait IDs mapped to Trait instances
        /// </summary>
        protected Dictionary<string, Trait> _traits;
        #endregion

        #region Properties
        /// <summary>
        /// Get Enumerable with all the traits in the library
        /// </summary>
        public IEnumerable<Trait> Traits => _traits.Values.ToList();
        #endregion

        #region Constructor
        public TraitLibrary()
        {
            _traits = new Dictionary<string, Trait>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add a trait to the library
        /// 
        /// <para>
        /// This method might overwrite and existing trait if they have the same TraitID
        /// </para>
        /// </summary>
        /// <param name="trait"></param>
        public void AddTrait(Trait trait)
        {
            _traits[trait.TraitID] = trait;
        }


        /// <summary>
        /// Retrieves an existing Trait using its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Trait GetTrait(string name)
        {
            return _traits[name];
        }
        #endregion
    }
}
