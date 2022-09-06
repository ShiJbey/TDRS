using System;
using System.Collections.Generic;
using UnityEngine;
using TraitBasedOpinionSystem.Core;
using System.Linq;

namespace TraitBasedOpinionSystem
{
    [CreateAssetMenu(fileName = "Trait", menuName = "Opinion System/Trait")]
    public class TraitScriptableObject : ScriptableObject
    {
        [Serializable]
        public struct OpinionModifierInfo {
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
        protected string displayName;

        [SerializeField]
        protected string description;

        [SerializeField]
        protected List<OpinionModifierInfo> modifiers;

        public string GetName()
        {
            return displayName;
        }

        public string GetDescription()
        {
            return description;
        }

        public List<OpinionModifierInfo> GetModifiers()
        {
            return modifiers;
        }

        public TraitType<object> CreateTraitType()
        {
            return new TraitType<object>(displayName, description, modifiers.Select(m => m.CreateOpinionModifier()).ToList());
        }

        public void Awake()
        {
            TraitTypeLibrary.AddTrait(CreateTraitType());
        }
    }

}
