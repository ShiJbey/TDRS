using System;
using TraitBasedOpinionSystem.Core;
using UnityEngine;

namespace TraitBasedOpinionSystem.Unity
{
    public static class UnityUtilities
    {
        public static void LogTraits(Actor actor)
        {
            foreach (var trait in actor.GetTraits())
            {
                Debug.Log(trait.GetName());
                foreach (var modifier in trait.GetModifiers())
                {
                    Debug.Log($"== {modifier.ToString()}");
                }
            }
        }
    }
}



