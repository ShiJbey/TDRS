using System;
using System.Collections.Generic;

namespace TraitBasedOpinionSystem.Core
{
    public class TraitNotFoundError : Exception
    {
        public readonly string traitName;

        public TraitNotFoundError(string traitName)
            : base($"No trait found with name: {traitName}")
        {
            this.traitName = traitName;
        }
    }

    public static class TraitLibrary
    {
        private static Dictionary<string, Trait<object>> _library =
            new Dictionary<string, Trait<object>>();

        public static void AddTrait(Trait<object> trait)
        {
            _library.Add(trait.Name, trait);
        }

        public static Trait<object> GetTrait(string name)
        {
            try
            {
                return _library[name];
            }
            catch (KeyNotFoundException)
            {
                throw new TraitNotFoundError(name);
            }
        }
    }
}
