using System.Collections.Generic;

namespace TraitBasedOpinionSystem
{
    /// <summary>
    /// Rules that are applied when a character is calculating their opinion of
    /// another. Rules have preconditions that must be met before they may be
    /// applied.
    /// </summary>
    public class SocialRule: ISocialRule
    {
        /// <summary>
        /// Preconnditions functions that must evaluate to true for the modifer
        /// to be applied
        /// </summary>
        protected readonly List<SocialRulePrecondition> _preconditions;

        /// <summary>
        /// Modifier value applied to the opinion
        /// </summary>
        protected readonly int _modifier;

        /// <summary>
        /// Unique name to identify this rule
        /// </summary>
        protected readonly string _name;

        /// <summary>
        /// Text description that can be shown to players
        /// </summary>
        protected readonly string _description;

        public SocialRule(
            string name,
            List<SocialRulePrecondition> preconditions,
            int modifier
            )
        {
            _name = name;
            _preconditions = preconditions;
            _modifier = modifier;
            _description = "";
        }


        public SocialRule(
            string name,
            string description,
            List<SocialRulePrecondition> preconditions,
            int modifier
            )
        {
            _name = name;
            _preconditions = preconditions;
            _modifier = modifier;
            _description = description;
        }

        public string Name {  get { return _name; } }

        public string Description { get { return _description; } }

        public List<SocialRulePrecondition> Preconditions { get { return _preconditions; } }

        public int GetModifier() { return _modifier; }

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
    }
}
