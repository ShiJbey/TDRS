using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace TraitBasedOpinionSystem
{
    [CreateAssetMenu(fileName = "Social Rule", menuName = "Opinion System/Social Rule")]
    public class SocialRuleScriptableObject : ScriptableObject
    {

        /// <summary>
        /// Unique name to identify this rule
        /// </summary>
        [SerializeField]
        protected string _name;

        /// <summary>
        /// Text description that can be shown to players
        /// </summary>
        [SerializeField]
        protected string _description;

        /// <summary>
        /// Preconditions functions that must evaluate to true for the modifier
        /// to be applied
        /// </summary>
        [SerializeField]
        protected List<string> _preconditions;

        /// <summary>
        /// Modifier value applied to the opinion
        /// </summary>
        [SerializeField]
        protected int _modifier;

        public string GetName()
        {
            return _name;
        }

        public string GetDescription()
        {
            return _name;
        }

        public SocialRulePrecondition[] GetPreconditions()
        {
            return _preconditions.Select<string, SocialRulePrecondition>((trait) =>
            {
                return (OpinionAgent subject, OpinionAgent target, Opinion opinion) =>
                {
                    return target.HasTrait(trait);
                };
            }).ToArray();
        }

        public int GetModifier()
        {
            return _modifier;
        }
    }
}
