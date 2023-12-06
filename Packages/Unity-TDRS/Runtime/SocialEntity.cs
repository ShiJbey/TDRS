using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDRS
{
    public abstract class SocialEntity : MonoBehaviour
    {
        #region Attributes
        /// <summary>
        /// A unique text identifier for this entity
        /// </summary>
        [SerializeField]
        protected string _entityID = "";

        /// <summary>
        /// The social rules that are active and affecting relationships
        /// </summary>
        protected List<SocialRule> _activeRules = new List<SocialRule>();

        /// <summary>
        /// Traits currentlt applied to the entity
        /// </summary>
        protected Dictionary<string, Trait> _traits = new Dictionary<string, Trait>();

        protected HashSet<string> _conflictingTraits = new HashSet<string>();

        /// <summary>
        /// Reference to the relationship manager
        /// </summary>
        protected RelationshipSystemManager? _relationshipManager = null;

        public List<string> traitsAtStart = new List<string>();
        #endregion

        #region Properties
        /// <summary>
        /// Get the unique ID of the entity
        /// </summary>
        public string EntityID => _entityID;

        /// <summary>
        /// Get the collection of traits attached to the entity
        /// </summary>
        public IEnumerable<Trait> Traits => _traits.Values.ToList();

        /// <summary>
        /// Get the collection of social rules affecting the entity's relationships
        /// </summary>
        public IEnumerable<SocialRule> ActiveRules => _activeRules;
        #endregion

        #region Unity Methods
        private void Start()
        {
            _relationshipManager = FindAnyObjectByType<RelationshipSystemManager>();

            if (_relationshipManager == null )
            {
                throw new System.Exception("Relationship manager not found.");
            }

            foreach(var traitID in traitsAtStart)
            {
                AddTrait(_relationshipManager.TraitLibrary.GetTrait(traitID));
            }
        }
        #endregion

        #region Trait Methods
        /// <summary>
        /// Add a trait to the entity
        /// </remarks>
        /// <param name="trait"></param>
        /// <returns></returns>
        public bool AddTrait(Trait trait)
        {
            if (_traits.ContainsKey(trait.TraitID))
            {
                return false;
            }

            if (HasConflictingTrait(trait))
            {
                return false;
            }

            _traits[trait.TraitID] = trait;

            _conflictingTraits.UnionWith(trait.ConflictingTraits);

            trait.OnAdd(this.gameObject);

            return true;
        }

        /// <summary>
        /// Remove a trait with a given ID
        /// </summary>
        /// <param name="traitID"></param>
        /// <returns></returns>
        public bool RemoveTrait(string traitID)
        {
            if (!_traits.ContainsKey(traitID))
            {
                return false;
            }

            Trait trait  = _traits[traitID];

            trait.OnRemove(this.gameObject);

            _conflictingTraits.Clear();
            foreach (var (_, remainingTrait) in _traits)
            {
                _conflictingTraits.UnionWith(remainingTrait.ConflictingTraits);
            }

            return true;
        }

        /// <summary>
        /// Return true if this entity has a trait with the given ID
        /// </summary>
        /// <param name="traitID"></param>
        /// <returns></returns>
        public bool HasTrait(string traitID)
        {
            return _traits.ContainsKey(traitID);
        }
        
        public bool HasConflictingTrait(Trait trait)
        {
            return _conflictingTraits.Contains(trait.TraitID);
        }
        #endregion

        #region SocialRule Methods
        /// <summary>
        /// Add a rule to the entities collection of active rules.
        /// </summary>
        /// <param name="rule"></param>
        public abstract void AddSocialRule(SocialRule rule);

        public abstract void RemoveSocialRule(SocialRule rule);

        /// <summary>
        /// Removes all social rules from the entity that have the given source
        /// </summary>
        /// <param name="source"></param>
        public abstract void RemoveAllSocialRulesFromSource(object source);

        public bool HasSocialRule(SocialRule rule)
        {
            return _activeRules.Contains(rule);
        }
        #endregion
    }
}

