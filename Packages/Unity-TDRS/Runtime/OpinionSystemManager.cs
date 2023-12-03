using System.Collections.Generic;
using UnityEngine;

namespace TraitBasedOpinionSystem
{
    /// <summary>
    /// Manages the libraries of social rule and character trait content.
    ///
    /// This class is to be used as a Singleton.
    /// </summary>
    public class OpinionSystemManager : MonoBehaviour
    {
        protected SocialRuleLibrary _socialRuleLibrary;
        protected TraitLibrary _traitLibrary;

        public TraitLibrary Traits { get { return _traitLibrary; } }
        public SocialRuleLibrary SocialRules { get { return _socialRuleLibrary; } }


        /// <summary>
        /// The OpinionSystemManager Awake method is responsible for initializing
        /// all the resources required (social rules, preconditions, traits)
        /// </summary>
        public void Awake()
        {
            _socialRuleLibrary = new SocialRuleLibrary();
            _traitLibrary = new TraitLibrary();
        }
    }
}
