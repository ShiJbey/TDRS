using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TraitBasedOpinionSystem.Core
{
    public class OpinionModifierLibrary
    {
        private static Dictionary<string, OpinionModifier> _library =
            new Dictionary<string, OpinionModifier>();

        public static void AddModifier(OpinionModifier modifier)
        {
            //OpinionModifierLibrary._library.Add(modifier.name, modifier);
        }

        public static OpinionModifier GetTrait(string name)
        {
            return OpinionModifierLibrary._library[name];
        }
    }
}
