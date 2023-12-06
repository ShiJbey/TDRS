using System.Collections.Generic;
using UnityEngine;


namespace TDRS
{
    [CreateAssetMenu(fileName = "Social Rule", menuName = "Opinion System/Social Rule")]
    public class SocialRuleScriptableObject : ScriptableObject
    {
        /// <summary>
        /// A name for to be displayed in GUIs
        /// </summary>
        public string displayName = "";

        /// <summary>
        /// Text description that can be shown to players
        /// </summary>
        [SerializeField]
        protected string description = "";

        /// <summary>
        /// Preconditions functions that must evaluate to true for the modifier
        /// to be applied
        /// </summary>
        [SerializeField]
        protected List<IPrecondition> preconditions = new List<IPrecondition>();

        /// <summary>
        /// Modifier value applied to the opinion
        /// </summary>
        [SerializeField]
        protected int _modifier = 0;

        public string GetName()
        {
            return displayName;
        }

        public string GetDescription()
        {
            return description;
        }

        public IEnumerable<IPrecondition> GetPreconditions()
        {
            return new IPrecondition[0];
        }

        public int GetModifier()
        {
            return _modifier;
        }
    }
}
