using System.Collections.Generic;
using UnityEngine;
using TraitBasedOpinionSystem.Core;

namespace TraitBasedOpinionSystem.Unity
{
    public class OpinionAgent : MonoBehaviour
    {
        private OpinionSystemManager _opinionSystem;
        private Actor _actor;

        [SerializeField]
        private bool _createOnStart;

        [SerializeField]
        private List<string> _traits;

        void Start()
        {
            if (_createOnStart)
            {
                _opinionSystem = FindObjectOfType<OpinionSystemManager>();
                _actor = _opinionSystem.OpinionSystem.SpawnActor();

                foreach (var traitName in this._traits)
                {
                    _actor.AddTrait(TraitTypeLibrary.GetTrait(traitName).Instantiate());
                }
            }
        }

        public Actor GetActor()
        {
            return _actor;
        }
    }
}
