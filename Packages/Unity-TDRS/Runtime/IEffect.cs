using UnityEngine;

namespace TDRS
{
    /// <summary>
    /// An Effect is modifies the state of a GameObject as part of a trait
    /// or other effect.
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// Get a string description of the effect.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Apply this effect to a target object.
        /// </summary>
        /// <param name="target"></param>
        public void Apply(GameObject target);

        /// <summary>
        /// Remove this effect from a target object.
        /// </summary>
        /// <param name="target"></param>
        public void Remove(GameObject target);
    }
}
