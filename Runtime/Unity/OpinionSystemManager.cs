using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TraitBasedOpinionSystem.Core;

namespace TraitBasedOpinionSystem.Unity
{
    public class OpinionSystemManager : MonoBehaviour
    {
        private OpinionSystem _opinionSystem = new OpinionSystem();

        [SerializeField]
        private List<TraitTypeScriptableObject> _traitTypes;

        public OpinionSystem OpinionSystem { get { return _opinionSystem; } }

        public void Awake()
        {
            RegisterTraitTypes();
        }

        private void RegisterTraitTypes()
        {
            foreach (var traitTypeSO in _traitTypes)
            {
                TraitTypeLibrary.AddTrait(traitTypeSO.CreateTraitType());
            }
        }
    }
}
