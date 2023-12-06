using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDRS
{
    public class SocialCharacter: SocialEntity
    {
        #region SocialRule Methods
        /// <summary>
        /// Add a rule to the entities collection of active rules.
        /// </summary>
        /// <param name="rule"></param>
        public override void AddSocialRule(SocialRule rule)
        {
            if (_relationshipManager != null)
            {
                throw new System.Exception("SocialEntity's relationship manager is null.");
            }

            _activeRules.Add(rule);

            IEnumerable<SocialRelationship> relationships;
            if (rule.IsOutgoing)
            {
                relationships = _relationshipManager.GetOutgoingRelationships(this);
            }
            else
            {
                relationships = _relationshipManager.GetIncomingRelationships(this);
            }

            foreach (var relationship in relationships)
            {
                if (rule.CheckPreconditions(relationship.gameObject))
                {
                    relationship.AddSocialRule(rule);
                    rule.OnAdd(relationship.gameObject);
                }
            }
        }

        public override void RemoveSocialRule(SocialRule rule)
        {
            if (!HasSocialRule(rule))
            {
                return;
            }

            if (_relationshipManager != null)
            {
                throw new System.Exception("SocialEntity's relationship manager is null.");
            }

            IEnumerable<SocialRelationship> relationships;
            if (rule.IsOutgoing)
            {
                relationships = _relationshipManager.GetOutgoingRelationships(this);
            }
            else
            {
                relationships = _relationshipManager.GetIncomingRelationships(this);
            }

            foreach (var relationship in relationships)
            {
                if (relationship.HasSocialRule(rule))
                {
                    rule.OnRemove(relationship.gameObject);
                    relationship.RemoveSocialRule(rule);
                }
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
                    // Get all the entities relationships and check them for the rule

                    _activeRules.RemoveAt(i);
                }
            }
        }
        #endregion
    }
}
