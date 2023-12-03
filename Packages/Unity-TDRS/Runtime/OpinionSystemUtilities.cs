using System.Collections.Generic;
using System.Linq;

namespace TraitBasedOpinionSystem
{
    public static class OpinionSystemUtilities
    {
        public static SocialRulePrecondition TargetHasTrait(string trait)
        {
            return (OpinionAgent subject, OpinionAgent target, Opinion opinion) =>
            {
                return target.HasTrait(trait);
            };
        }

        public static SocialRulePrecondition SubjectHasTrait(string trait)
        {
            return (OpinionAgent subject, OpinionAgent target, Opinion opinion) =>
            {
                return subject.HasTrait(trait);
            };
        }

        public static SocialRulePrecondition OR(SocialRulePrecondition[] preconditions)
        {
            return (OpinionAgent subject, OpinionAgent target, Opinion opinion) =>
            {
                foreach (var p in preconditions)
                {
                    if (p(subject, target, opinion)) return true;
                }
                return false;
            };
        }

        public static SocialRulePrecondition AND(SocialRulePrecondition[] preconditions)
        {
            return (OpinionAgent subject, OpinionAgent target, Opinion opinion) =>
            {
                foreach (var p in preconditions)
                {
                    if (!p(subject, target, opinion)) return false;
                }
                return true;
            };
        }

        public static Dictionary<TKey, TValue>
                 Merge<TKey, TValue>(Dictionary<TKey, TValue>[] dictionaries)
        {
            return dictionaries.SelectMany(dict => dict)
                         .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}

