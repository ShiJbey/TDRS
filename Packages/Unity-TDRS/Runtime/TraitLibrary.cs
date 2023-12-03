using System.Collections.Generic;
using static UnityEngine.ParticleSystem;

namespace TraitBasedOpinionSystem
{
    /// <summary>
    /// This factory implements the flyweight design pattern and ensures that
    /// all the instances of Traits are unique.
    ///
    /// Please note that if traits are not constructed using a factory or
    /// constructed using a different factory instances may not be unique
    /// </summary>
    public class TraitLibrary
    {
        protected Dictionary<string, Trait> _traits = new Dictionary<string, Trait>();

        /// <summary>
        /// Retrieves an existing Trait using its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Trait GetByName(string name)
        {
            return _traits[name.ToLower()];
        }

        /// <summary>
        /// Returns an existing trait instance or creates a new one using the
        /// given parameters.
        ///
        /// The given name is case-insensitive
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        public Trait Get(string name, string description, ISocialRule[] rules)
        {
            if (!_traits.ContainsKey(name.ToLower()))
            {
                _traits[name.ToLower()] = new Trait(name, description, rules);
            }
            return _traits[name.ToLower()];
        }

        /// <summary>
        /// Returns a new Trait overwriting any existing traits
        ///
        /// The given name is case-insensitive
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        public Trait OverwriteExisting(string name, string description, ISocialRule[] rules)
        {
            _traits[name.ToLower()] = new Trait(name, description, rules);
            return _traits[name.ToLower()];
        }
    }
}
