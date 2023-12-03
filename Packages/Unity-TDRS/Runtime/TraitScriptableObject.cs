using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TraitBasedOpinionSystem
{
    [CreateAssetMenu(fileName = "Trait", menuName = "Opinion System/Trait")]
    public class TraitScriptableObject : ScriptableObject
    {

        [System.Serializable]
        public struct RuleInfo
        {
            public string[] preconditionTraits;
            public int modifier;
        }

        /// <summary>
        /// Name of the trait (must be unique and is used to identify the trait)
        /// </summary>
        [SerializeField]
        public string traitName;

        /// <summary>
        /// String description of the trait (mainly used in user interfaces)
        /// </summary>
        [SerializeField]
        protected string description;

        /// <summary>
        /// Social rules associated with this this trait
        /// </summary>
        [SerializeField]
        protected List<RuleInfo> socialRules;


        public string GetName()
        {
            return traitName;
        }

        public string GetDescription()
        {
            return description;
        }

        public TraitSocialRule[] GetRules()
        {
            var rules = new List<TraitSocialRule>();
            foreach(var ruleInfo in socialRules)
            {
                rules.Add(
                    new TraitSocialRule(
                        ruleInfo.modifier,
                        ruleInfo.preconditionTraits.Select<string, SocialRulePrecondition>(
                            (trait) => {
                                return (OpinionAgent subject, OpinionAgent target, Opinion opinion) =>
                                {
                                    return target.HasTrait(trait);
                                };
                            }).ToArray()));
            }
            return rules.ToArray();
        }
    }

}
