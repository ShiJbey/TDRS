using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TDRS
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

        #region Attributes
        /// <summary>
        /// A unique ID for this trait.
        /// </summary>
        public string traitID = "";

        /// <summary>
        /// Name of the Trait for use in GUIs.
        /// </summary>
        public string displayName = "";

        /// <summary>
        /// A textual description of the trait.
        /// </summary>
        public string description = "";
        #endregion

        // public TraitSocialRule[] GetRules()
        // {
        //     var rules = new List<TraitSocialRule>();
        //     foreach (var ruleInfo in socialRules)
        //     {
        //         rules.Add(
        //             new TraitSocialRule(
        //                 ruleInfo.modifier,
        //                 ruleInfo.preconditionTraits.Select<string, SocialRulePrecondition>(
        //                     (trait) =>
        //                     {
        //                         return (OpinionAgent subject, OpinionAgent target, Opinion opinion) =>
        //                         {
        //                             return target.HasTrait(trait);
        //                         };
        //                     }).ToArray()));
        //     }
        //     return rules.ToArray();
        // }
    }

}
