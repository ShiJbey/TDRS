using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDRS
{
    /// <summary>
    /// TraitTypes manage metadata associated with Traits.
    ///
    /// TraitTypes create instances of traits and allow the
    /// OpinionSystem to save memory by only having a single
    /// copy of trait metadata.
    /// </summary>
    public class Trait
    {
        #region Attributes
        /// <summary>
        /// A unique ID for this Trait.
        /// </summary>
        protected readonly string _traitID;
        /// <summary>
        /// An user-friendly name to use for GUIs.
        /// </summary>
        protected readonly string _displayName;
        /// <summary>
        /// A short description of the trait.
        /// </summary>
        protected readonly string _description;
        /// <summary>
        /// Effects to apply when the trait is added
        /// </summary>
        protected readonly List<IEffect> _effects;
        /// <summary>
        /// IDs of traits that this trait cannot be present with
        /// </summary>
        protected readonly HashSet<string> _conflictingTraits;
        #endregion

        #region Properties
        /// <summary>
        /// A unique ID for this Trait.
        /// </summary>
        public string TraitID => _traitID;
        /// <summary>
        /// An user-friendly name to use for GUIs.
        /// </summary>
        public string DisplayName => _displayName;
        /// <summary>
        /// A short description of the trait.
        /// </summary>
        public string Description => _description;
        /// <summary>
        /// Effects to apply when the trait is added
        /// </summary>
        public IEnumerable<IEffect> Effects => _effects;

        public HashSet<string> ConflictingTraits => _conflictingTraits;
        #endregion

        #region Constructors
        public Trait(
            string traitID,
            string displayName,
            string description,
            IEnumerable<IEffect> effects,
            HashSet<string> conflictingTraits
            )
        {
            _traitID = traitID;
            _displayName = displayName;
            _description = description;
            _effects = effects.ToList();
            _conflictingTraits = conflictingTraits;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Callback method executed when the trait is added to an object.
        /// </summary>
        /// <param name="target"></param>
        public void OnAdd(GameObject target)
        {
            foreach (var effect in _effects)
            {
                effect.Apply(target);
            }
        }

        /// <summary>
        /// Callback method executed when the trait is removed from an object.
        /// </summary>
        /// <param name="target"></param>
        public void OnRemove(GameObject target)
        {
            foreach (var effect in _effects)
            {
                effect.Remove(target);
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
        #endregion
    }
}
