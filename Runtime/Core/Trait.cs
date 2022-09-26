using System.Collections.Generic;
using System.Linq;

namespace TraitBasedOpinionSystem.Core
{
    /// <summary>
    /// Traits are core the this opinion system and are used to
    /// describe NPC personalities, physical appearances, or
    /// other attributes.
    ///
    /// Traits need to have a unique name. Additionally, they
    /// may include a short text description and a set of
    /// opinion modifiers that are tested when calculating one
    /// NPC's opinion of another
    /// </summary>
    public class Trait
    {
        /// <summary>
        /// The name of the TraitType associated with this trait
        /// </summary>
        protected readonly string _name;

        /// <summary>
        /// Modifiers associated with this trait
        /// </summary>
        protected readonly List<OpinionModifier> _modifiers;

        public Trait(string name)
        {
            _name = name;
            _modifiers = new List<OpinionModifier>();
        }

        public Trait(string name, List<OpinionModifier> modifiers)
        {
            _name = name;
            _modifiers = modifiers.ToList();
        }

        public string GetName()
        {
            return _name;
        }

        public List<OpinionModifier> GetModifiers()
        {
            return _modifiers;
        }
    }
}
