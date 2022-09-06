using System.Collections.Generic;
using UnityEngine;
using TraitBasedOpinionSystem.Core;

namespace TraitBasedOpinionSystem.Unity
{
    public class OpinionAgent : MonoBehaviour
    {
        private uint _agentID;
        
        private OpinionSystemManager _opinionSystem;

        [SerializeField]
        private List<string> _traits;

        void Start()
        {
            _opinionSystem = FindObjectOfType<OpinionSystemManager>();
            var actor = _opinionSystem.OpinionSystem.SpawnActor();
            _agentID = actor.ID;

            foreach (var traitName in this._traits)
            {
                actor.AddTrait(TraitLibrary.GetTrait(traitName));
            }
        }

        private void OnMouseUp()
        {
            Debug.Log($"Agent {_agentID} clicked.");
            var opinion = _opinionSystem.OpinionSystem.GetOpinion(
                _opinionSystem.OpinionSystem.GetActor(_agentID),
                _opinionSystem.OpinionSystem.GetActor(1));
            Debug.Log($"Agent-{_agentID} to Agent-{1}: {opinion.Value}.");
        }
    }

}
