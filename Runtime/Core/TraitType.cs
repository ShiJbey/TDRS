using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraitBasedOpinionSystem.Core
{
    /// <summary>
    /// TraitTypes manage metadata associated with Traits.
    ///
    /// TraitTypes create instances of traits and allow the
    /// OpinionSystem to save memory by only having a single
    /// copy of trait metadata.
    /// </summary>
    public class TraitType
    {

        protected readonly uint _id;
        protected readonly string _name;
        protected readonly string _description;
        protected readonly List<OpinionModifier> _modifiers;

        public TraitType(uint id, string name)
        {
            _id = id;
            _name = name;
            _description = "";
            _modifiers = new List<OpinionModifier>();
        }

        public TraitType(uint id, string name, IEnumerable<OpinionModifier> modifiers)
        {
            _id = id;
            _name = name;
            _description = "";
            _modifiers = modifiers.ToList();
        }

        public TraitType(uint id, string name, string description, IEnumerable<OpinionModifier> modifiers)
        {
            _id = id;
            _name = name;
            _description = description;
            _modifiers = modifiers.ToList();
        }

        public uint GetID()
        {
            return _id;
        }

        public string GetName()
        {
            return _name;
        }

        public string GetDescription()
        {
            return _description;
        }

        public Trait Instantiate()
        {
            return new Trait(_name, _modifiers);
        }
    }
}
