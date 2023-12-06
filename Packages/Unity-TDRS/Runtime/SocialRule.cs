using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;

namespace TDRS
{
    /// <summary>
    /// Rules that are applied when a character is calculating their opinion of
    /// another. Rules have preconditions that must be met before they may be
    /// applied.
    /// </summary>
    public class SocialRule
    {
        #region Attributes
        /// <summary>
        /// Preconditions that need to pass for the social rule to be applied
        /// </summary>
        protected readonly List<IPrecondition> _preconditions;
        
        /// <summary>
        /// Effects applied by the social rule if its preconditions pass
        /// </summary>
        protected readonly List<IEffect> _effects;

        /// <summary>
        /// is True if this rull is applied to outgoing relationships
        /// </summary>
        protected readonly bool _isOutgoing = true;

        /// <summary>
        /// The object responsible to creating and adding this rule to a collection
        /// </summary>
        protected readonly object? _source = null;
        #endregion

        #region Properties
        /// <summary>
        /// Preconditions that need to pass for the social rule to be applied
        /// </summary>
        public IEnumerable<IPrecondition> Preconditions => _preconditions;

        /// <summary>
        /// Effects applied by the social rule if its preconditions pass
        /// </summary>
        public IEnumerable<IEffect> Effects => _effects;

        /// <summary>
        /// is True if this rull is applied to outgoing relationships
        /// </summary>
        public bool IsOutgoing => _isOutgoing;

        /// <summary>
        /// The object responsible to creating and adding this rule to a collection
        /// </summary>
        public object? Source => _source;
        #endregion

        #region Constructors
        public SocialRule(
            IEnumerable<IPrecondition> preconditions,
            IEnumerable<IEffect> effects,
            bool outgoing = true,
            object? source = null
            )
        {
            _preconditions = preconditions.ToList();
            _effects = effects.ToList();
            _isOutgoing = outgoing;
            _source = source;
        }
        #endregion

        #region Methods
        public bool CheckPreconditions(GameObject relationship)
        {
            foreach (var precondition in _preconditions)
            {
                if (precondition.CheckPrecondition(relationship) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public void OnAdd(GameObject relationship)
        {
            foreach (var effect in _effects)
            {
                effect.Apply(relationship);
            }
        }

        public void OnRemove(GameObject relationship)
        {
            foreach (var effect in _effects)
            {
                effect.Remove(relationship);
            }
        }
        #endregion
    }
}
