using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TraitBasedOpinionSystem
{
    /// <summary>
    /// A social rule associated with a Trait instance
    /// </summary>
    public class TraitSocialRule : ISocialRule
    {
        protected int _modifier;
        protected SocialRulePrecondition[] _preconditions;


        public TraitSocialRule(int modifier)
        {
            _modifier = modifier;
            _preconditions = new SocialRulePrecondition[0];
        }


        public TraitSocialRule(
            int modifier,
            SocialRulePrecondition[] preconditions
            )
        {
            _modifier = modifier;
            _preconditions = preconditions;
        }

        public bool CheckPreconditions(OpinionAgent subject, OpinionAgent target, Opinion opinion)
        {
            foreach (var p in _preconditions)
            {
                if (p(subject, target, opinion) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public int GetModifier()
        {
            return _modifier;
        }
    }
}

