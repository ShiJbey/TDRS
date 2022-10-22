using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TraitBasedOpinionSystem
{
    /// <summary>
    /// Abstract base class for social rules
    /// </summary>
    public interface ISocialRule
    {
        /// <summary>
        /// Return the int modifier to the subject's opinion of the target
        /// </summary>
        /// <returns></returns>
        public abstract int GetModifier();

        /// <summary>
        /// Return true if the preconditions for applying this rule pass
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="target"></param>
        /// <param name="opinion"></param>
        /// <returns></returns>
        public abstract bool CheckPreconditions(OpinionAgent subject, OpinionAgent target, Opinion opinion);
    }
}

