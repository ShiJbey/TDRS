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
    public class Trait<T>
    {
        protected readonly string _name;
        protected T _value;
        protected readonly List<OpinionModifier> _modifiers;

        /// <summary>
        /// Unique name of the trait
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Value held by this trait
        /// </summary>
        public T Value { 
            get {
                return _value;
            }
        }

        /// <summary>
        /// Associated OpinionModifiers that are tested on opinion calculation
        /// </summary>
        public List<OpinionModifier> Modifiers { get { return _modifiers; } }

        public Trait(string name)
        {
            _name = name;
            _value = default(T);
            _modifiers = new List<OpinionModifier>();
        }

        public Trait(string name, T value)
        {
            _name = name;
            _value = value;
            _modifiers = new List<OpinionModifier>();
        }

        public Trait(string name, List<OpinionModifier> modifiers)
        {
            _name = name;
            _value = default(T);
            _modifiers = modifiers.ToList();
        }

        public Trait(string name, T value, List<OpinionModifier> modifiers)
        {
            _name = name;
            _value = value;
            _modifiers = modifiers.ToList();
        }
    }
}
