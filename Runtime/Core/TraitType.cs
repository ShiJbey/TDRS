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
    /// copy of trait metadata like descriptions.
    /// </summary>
    public class TraitType<T, TraitValue>
    {

        protected string _name;
        protected string _description;
        protected List<OpinionModifier> _modifiers;

        public TraitType(string name)
        {
            _name = name;
            _description = "";
            _modifiers = new List<OpinionModifier>();
        }

        public TraitType(string name, IEnumerable<OpinionModifier> modifiers)
        {
            _name = name;
            _description = "";
            _modifiers = modifiers.ToList();
        }

        public TraitType(string name, string description, IEnumerable<OpinionModifier> modifiers)
        {
            _name = name;
            _description = description;
            _modifiers = modifiers.ToList();
        }

        public string GetName()
        {
            return _name;
        }

        public string GetDescription()
        {
            return _description;
        }

        public Trait<T> Instantiate(T value)
        {
            return new Trait<T>(_name, value, _modifiers);
        }
    }
}
