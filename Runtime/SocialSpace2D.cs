using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TraitBasedOpinionSystem
{
    public class SocialSpace2D : MonoBehaviour
    {
        /// <summary>
        /// Reference to trigger collider component
        /// </summary>
        protected Collider2D _collider;

        /// <summary>
        /// SocialRules specified using scriptable objects
        /// </summary>
        [SerializeField]
        protected List<SocialRuleScriptableObject> _socialRules =
            new List<SocialRuleScriptableObject>();

        /// <summary>
        /// SocialRule instances created from the ScriptableObjects
        /// passed in the inspector
        /// </summary>
        protected List<SocialRule> _ruleInstances = new List<SocialRule>();

        private void Start()
        {
            _collider = gameObject.GetComponent<Collider2D>();
            LoadRules();
        }

        /// <summary>
        /// Loads SocialRule instances from ScriptableObjects in the inspector
        /// </summary>
        private void LoadRules()
        {
            var opinionSystemManager = FindObjectOfType<OpinionSystemManager>();

            foreach (var ruleInfo in _socialRules)
            {
                AddRule(opinionSystemManager.SocialRules.Get(
                    ruleInfo.GetName(),
                    ruleInfo.GetDescription(),
                    ruleInfo.GetModifier(),
                    ruleInfo.GetPreconditions()));
            }
        }

        public void AddRule(SocialRule rule)
        {
            _ruleInstances.Add(rule);
        }


        public List<SocialRule> Rules
        {
            get { return _ruleInstances; }
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            var agent = other.gameObject.GetComponent<OpinionAgent>();
            if (agent != null)
            {
                foreach (var rule in _ruleInstances)
                {
                    agent.AddLocalRule(rule);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var agent = other.gameObject.GetComponent<OpinionAgent>();
            if (agent != null)
            {
                foreach (var rule in _ruleInstances)
                {
                    agent.RemoveLocalRule(rule);
                }
            }
        }
    }
}
