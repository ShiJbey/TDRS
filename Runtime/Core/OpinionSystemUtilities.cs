using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TraitBasedOpinionSystem.Core
{
    public static class OpinionSystemUtilities
    {
        public static OpinionModifierPrecondition TargetHasTrait(string trait)
        {
            return (Actor subject, Actor target, Opinion opinion) =>
            {
                return target.HasTrait(trait);
            };
        }

        public static OpinionModifierPrecondition OR(OpinionModifierPrecondition[] preconditions)
        {
            return (Actor subject, Actor target, Opinion opinion) =>
            {
                foreach (var p in preconditions)
                {
                    if (p(subject, target, opinion)) return true;
                }
                return false;
            };
        }

        public static OpinionModifierPrecondition AND(OpinionModifierPrecondition[] preconditions)
        {
            return (Actor subject, Actor target, Opinion opinion) =>
            {
                foreach (var p in preconditions)
                {
                    if (!p(subject, target, opinion)) return false;
                }
                return true;
            };
        }
    }
}

