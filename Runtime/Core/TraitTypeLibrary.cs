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
        private static Dictionary<string, TraitType> _library =
            new Dictionary<string, TraitType>();

        public static void AddTrait(TraitType traitType)
        {
            _library.Add(traitType.GetName(), traitType);
        }

        public static TraitType GetTrait(string traitName)
        {
            try
            {
                return _library[traitName];
            }
            catch (KeyNotFoundException)
            {
                throw new TraitTypeNotFoundError(traitName);
            }
        }
    }
}
