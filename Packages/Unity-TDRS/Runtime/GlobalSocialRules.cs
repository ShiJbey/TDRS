using System.Collections.Generic;
using UnityEngine;


namespace TraitBasedOpinionSystem
{
    /// <summary>
    /// Component that specifies social rules that apply to all characters
    /// within the scene. Characters look for an instance of this class when
    /// calculating their opinions.
    /// </summary>
    public class GlobalSocialRules : MonoBehaviour
    {
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
    }
}
