using System.Collections.Generic;
using System.Linq;

namespace TraitBasedOpinionSystem.Core
{
    public class Actor
    {
        protected readonly Dictionary<string, Trait<object>> _traits;
        protected readonly uint _id;

        public Actor(uint id)
        {
            _id = id;
            _traits = new Dictionary<string, Trait<object>>();
        }

        /// <summary>
        /// The Actor's unique identifier
        /// </summary>
        public uint ID { get { return _id; } }

        /// <summary>
        /// List of all Traits attached to this actor
        /// </summary>
        public List<Trait<object>> Traits {
            get { return _traits.Values.ToList();  }
        }

        /// <summary>
        /// Adds a trait to the actor
        /// </summary>
        /// <remarks>
        /// This method will not overwrite an existing trait
        /// with the name name
        /// </remarks>
        /// <param name="trait"></param>
        public void AddTrait(Trait<object> trait)
        {
            _traits.Add(trait.Name, trait);
        }

        /// <summary>
        /// Remove a trait with a given name
        /// </summary>
        /// <param name="traitName"></param>
        public void RemoveTrait(string traitName)
        {
            _traits.Remove(traitName);
        }

        /// <summary>
        /// Return true if this actor has a trait with the given name
        /// </summary>
        /// <param name="traitName"></param>
        /// <returns></returns>
        public bool HasTrait(string traitName)
        {
            return _traits.ContainsKey(traitName);
        }
    }
}
