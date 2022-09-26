using System;
using System.Collections.Generic;
using UnityEngine;
using TraitBasedOpinionSystem.Core;
using System.Linq;

namespace TraitBasedOpinionSystem.Unity
{
    [CreateAssetMenu(fileName = "Trait", menuName = "Opinion System/Trait")]
    public class TraitTypeScriptableObject : ScriptableObject
    {
        [Serializable]
        public struct OpinionModifierInfo
        {
            [SerializeField]
            string traitName;

            [SerializeField]
            int value;

            public string ToSting()
            {
                return $"Opinion of {traitName} characters: {value}";
            }

            public OpinionModifier CreateOpinionModifier()
            {
                return new HasTraitModifier(traitName, value);
            }
        }

        [SerializeField]
        protected uint id;

        [SerializeField]
        protected string traitName;

        [SerializeField]
        protected string description;

        [SerializeField]
        protected List<OpinionModifierInfo> modifiers;

        public string GetName()
        {
            return traitName;
        }

        public string GetDescription()
        {
            return description;
        }

        public List<OpinionModifierInfo> GetModifiers()
        {
            return modifiers;
        }

        public TraitType CreateTraitType()
        {
            return new TraitType(id, traitName, description, modifiers.Select(m => m.CreateOpinionModifier()).ToList());
        }
    }

}
