using System.Linq;
using System.Collections.Generic;

namespace TraitBasedOpinionSystem
{
    /// <summary>
    /// This factory implements the flyweight design pattern and ensures that
    /// all the instances of SocialRules are unique.
    ///
    /// Please note that if Social Rules that are not constructed using a
    /// factory or constructed using a different factory instances may not be
    /// unique
    /// </summary>
    public class SocialRuleLibrary
    {
        protected Dictionary<string, SocialRule> _rules = new Dictionary<string, SocialRule>();

        /// <summary>
        /// Retrieves an existing Trait using its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SocialRule GetByName(string name)
        {
            return _rules[name.ToLower()];
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
        public SocialRule Get(
            string name,
            string description,
            int modifier,
            SocialRulePrecondition[] preconditions
            )
        {
            if (!_rules.ContainsKey(name.ToLower()))
            {
                _rules[name.ToLower()] = new SocialRule(name, description, preconditions.ToList(), modifier);
            }
            return _rules[name.ToLower()];
        }

        /// <summary>
        /// Returns a new Trait overwriting any existing traits
        ///
        /// The given name is case-insensitive
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="modifier"></param>
        /// <param name="preconditions"></param>
        /// <returns></returns>
        public SocialRule OverwriteExisting(
            string name,
            string description,
            int modifier,
            SocialRulePrecondition[] preconditions
            )
        {
            _rules[name.ToLower()] = new SocialRule(name, description, preconditions.ToList(), modifier);
            return _rules[name.ToLower()];
        }
    }
}
