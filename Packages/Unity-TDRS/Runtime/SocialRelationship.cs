using System.Collections.Generic;
using TDRS.StatSystem;

namespace TDRS
{
    public class SocialRelationship : SocialEntity
    {
        // These values are used to clamp stat values
        public static readonly float STAT_MAX = 100f;
        public static readonly float STAT_MIN = -100f;

        /// <summary>
        /// Character that owns this relationship
        /// </summary>
        protected SocialEntity? _owner = null;

        /// <summary>
        /// Character that this relationship is directed toward
        /// </summary>
        protected SocialEntity? _target = null;

        /// <summary>
        /// Relationship stats names mapped to Stat instances
        /// </summary>
        protected Dictionary<string, Stat> _stats = new Dictionary<string, Stat>();

        public SocialEntity? Owner => _owner;

        public SocialEntity? Target => _target;

        public Dictionary<string, Stat> Stats => _stats;

        #region SocialRule Methods
        /// <summary>
        /// Add a rule to the entities collection of active rules.
        /// </summary>
        /// <param name="rule"></param>
        public override void AddSocialRule(SocialRule rule)
        {
            _activeRules.Add(rule);
        }

        public override void RemoveSocialRule(SocialRule rule)
        {
            if (!HasSocialRule(rule))
            {
                return;
            }

            _activeRules.Remove(rule);
        }

        /// <summary>
        /// Removes all social rules from the entity that have the given source
        /// </summary>
        /// <param name="source"></param>
        public override void RemoveAllSocialRulesFromSource(object source)
        {
            // Loop backward through the social rules and remove all that have the given source
            for (int i = _activeRules.Count; i > 0; i--)
            {
                var rule = _activeRules[i];

                if (rule.Source == source)
                {
                    _activeRules.RemoveAt(i);
                }
            }
        }
        #endregion
    }
}


