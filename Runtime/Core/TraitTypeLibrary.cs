using System;
using System.Collections.Generic;

namespace TraitBasedOpinionSystem.Core
{
    public class TraitTypeNotFoundError : Exception
    {
        public readonly string traitName;

        public TraitTypeNotFoundError(string traitName)
            : base($"No TraitType found with name: {traitName}")
        {
            this.traitName = traitName;
        }
    }

    public static class TraitTypeLibrary
    {
        private static Dictionary<string, TraitType<object>> _library =
            new Dictionary<string, TraitType<object>>();

        public static void AddTrait(TraitType<object> traitType)
        {
            _library.Add(traitType.GetName(), traitType);
        }

        public static TraitType<object> GetTrait(string traitName)
        {
            try
            {
                return _library[traitName];
            } catch (KeyNotFoundException)
            {
                throw new TraitTypeNotFoundError(traitName);
            }
        }
    }
}
