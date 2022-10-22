using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

namespace TraitBasedOpinionSystem
{
    /// <summary>
    /// Opinion Agents have traits and form opinions about other
    /// other characters based on other character's traits, and global/local
    /// social rules.
    /// </summary>
    public class OpinionAgent : MonoBehaviour
    {
        /// <summary>
        /// Traits specified by the designer in Unity's inspector
        /// </summary>
        [Header("Character Traits")]
        [SerializeField]
        protected List<TraitScriptableObject> _traitScriptableObjects = new List<TraitScriptableObject>();

        /// <summary>
        /// Traits that this character has attached.
        ///
        /// This may differ from the _traitScriptableObjects list if traits are
        /// added from code at runtime instead of via Unity's inspector
        /// </summary>
        protected Dictionary<string, Trait> _traits = new Dictionary<string, Trait>();

        /// <summary>
        /// Character UIDs mapped to the opinion this character has about the
        /// characters who owns the UID.
        /// </summary>
        protected Dictionary<OpinionAgent, Opinion> _opinions = new Dictionary<OpinionAgent, Opinion>();

        /// <summary>
        /// The social rules that a character is subject to given it's location
        /// </summary>
        protected List<SocialRule> _localRules = new List<SocialRule>();

        /// <summary>
        /// Reference to Global Social Rules this character follows;
        /// </summary>
        protected GlobalSocialRules _globalRules;

        /// <summary>
        /// Loads Trait instances from ScriptableObjects in the inspector
        /// </summary>
        private void LoadTraits()
        {
            var opinionSystemManager = FindObjectOfType<OpinionSystemManager>();

            foreach (var traitInfo in _traitScriptableObjects)
            {
                AddTrait(opinionSystemManager.Traits.Get(
                    traitInfo.GetName(),
                    traitInfo.GetDescription(),
                    traitInfo.GetRules()));
            }
        }

        /// <summary>
        /// Checks for the presence of GlobalSocialRules and adds them
        /// to the rules used during opinion calculation
        /// </summary>
        private void InheritGlobalSocialRules()
        {
            _globalRules = FindObjectOfType<GlobalSocialRules>();
        }

        /// <summary>
        /// Recalculate all opinions this character has about other characters
        /// </summary>
        public void RecalculateAllOpinions()
        {
            foreach (var item in _opinions)
            {
                if (_globalRules != null)
                {
                    item.Value.Recalculate(_globalRules.Rules);
                }
                else
                {
                    item.Value.Recalculate();
                }
            }
        }

        private void Start()
        {
            LoadTraits();
            InheritGlobalSocialRules();
        }

        /// <summary>
        /// List of all Traits attached to this actor
        /// </summary>
        public List<Trait> GetTraits()
        {
            return _traits.Values.ToList();
        }

        /// <summary>
        /// Adds a trait to the actor
        /// </summary>
        /// <remarks>
        /// This method will not overwrite an existing trait
        /// with the name name
        /// </remarks>
        /// <param name="trait"></param>
        public void AddTrait(Trait trait)
        {
            _traits.Add(trait.Name.ToLower(), trait);
        }

        /// <summary>
        /// Remove a trait with a given name
        /// </summary>
        /// <param name="traitName"></param>
        public void RemoveTrait(string traitName)
        {
            _traits.Remove(traitName.ToLower());
        }

        /// <summary>
        /// Return true if this actor has a trait with the given name
        /// </summary>
        /// <param name="traitName"></param>
        /// <returns></returns>
        public bool HasTrait(string traitName)
        {
            return _traits.ContainsKey(traitName.ToLower());
        }

        /// <summary>
        /// Return this character's opinion of another
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Opinion GetOpinion(OpinionAgent target)
        {
            if (!_opinions.ContainsKey(target))
            {
                _opinions.Add(target, new Opinion(this, target));
            }

            if (_globalRules != null)
            {
                _opinions[target].Recalculate(_globalRules.Rules);
            }
            else
            {
                _opinions[target].Recalculate();
            }

            return _opinions[target];
        }

        public void AddLocalRule(SocialRule rule)
        {
            _localRules.Add(rule);
        }

        public void RemoveLocalRule(SocialRule rule)
        {
            _localRules.Remove(rule);
        }

        public List<SocialRule> GetLocalRules()
        {
            return _localRules;
        }
    }
}
