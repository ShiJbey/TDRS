using System;
using System.Collections.Generic;

namespace TraitBasedOpinionSystem
{
    public class Opinion
    {
        // These values are used to clamp opinion values
        public static readonly int OPINION_MAX = 100;
        public static readonly int OPINION_MIN = -100;

        /// <summary>
        /// Character that owns this opinion
        /// </summary>
        protected OpinionAgent _subject;

        /// <summary>
        /// Character that this opinion is directed toward
        /// </summary>
        protected OpinionAgent _target;

        /// <summary>
        /// Starting opinion value independent of any other values
        /// </summary>
        protected int _baseValue;

        /// <summary>
        /// The opinion score clamped between OPINION_MAX and OPINION_MIN
        /// </summary>
        protected int _value;

        /// <summary>
        /// The raw opinion score without clamping
        /// </summary>
        protected int _rawValue;

        /// <summary>
        /// Normalized opinion score [0, 1.0], with complete dislike = 0,
        /// meh = 0.5, love = 1.0
        /// </summary>
        protected float _normalizedValue;

        /// <summary>
        /// The social rules that are active in determining this opinion score
        /// </summary>
        protected List<ISocialRule> _activeRules;

        public Opinion(OpinionAgent subject, OpinionAgent target, int baseValue = 0)
        {
            _subject = subject;
            _target = target;
            _baseValue = baseValue;
            _value = Math.Clamp(baseValue, OPINION_MIN, OPINION_MAX);
            _rawValue = baseValue;
            _normalizedValue = 0.5f;
            _activeRules = new List<ISocialRule>();
        }

        /// <summary>
        /// Returns opinion score clamped on interval [-100, 100]
        /// </summary>
        public int GetValue()
        {
            return _value;
        }

        /// <summary>
        /// Returns raw opinion score without clamping
        /// </summary>
        public int GetRawValue()
        {
            return _rawValue;
        }

        /// <summary>
        /// Returns normalized opinion score [0, 1.0]
        /// </summary>
        public float GetNormalizedValue()
        {
            return _normalizedValue;
        }

        /// <summary>
        /// Recalculates this characters opinion of a specific character
        /// </summary>
        public void Recalculate(List<SocialRule> globalRules = null)
        {
            int boost = 0;
            float totalPositiveOpinion = 0f;
            float totalNegativeOpinion = 0f;

            _activeRules.Clear();

            foreach (var trait in _subject.GetTraits())
            {
                foreach (var rule in trait.Rules)
                {
                    if (rule.CheckPreconditions(_subject, _target, this))
                    {
                        _activeRules.Add(rule);

                        int modifier = rule.GetModifier();

                        boost += modifier;

                        // Check the sign
                        if (modifier < 0)
                        {
                            totalNegativeOpinion += Math.Abs(modifier);
                        }
                        else
                        {
                            totalPositiveOpinion += modifier;
                        }
                    }
                }
            }

            foreach (var rule in _subject.GetLocalRules())
            {
                if (rule.CheckPreconditions(_subject, _target, this))
                {
                    _activeRules.Add(rule);

                    int modifier = rule.GetModifier();

                    boost += modifier;

                    // Check the sign
                    if (modifier < 0)
                    {
                        totalNegativeOpinion += Math.Abs(modifier);
                    }
                    else
                    {
                        totalPositiveOpinion += modifier;
                    }
                }
            }

            if (globalRules != null)
            {
                foreach (var rule in globalRules)
                {
                    if (rule.CheckPreconditions(_subject, _target, this))
                    {
                        _activeRules.Add(rule);

                        int modifier = rule.GetModifier();

                        boost += modifier;

                        // Check the sign
                        if (modifier < 0)
                        {
                            totalNegativeOpinion += Math.Abs(modifier);
                        }
                        else
                        {
                            totalPositiveOpinion += modifier;
                        }
                    }
                }
            }

            _value = Math.Clamp(_baseValue + boost, OPINION_MIN, OPINION_MAX);
            _rawValue = _baseValue + boost;

            if (totalPositiveOpinion + totalNegativeOpinion == 0)
            {
                _normalizedValue = 0.5f;
            }
            else
            {
                _normalizedValue = totalPositiveOpinion / (totalPositiveOpinion + totalNegativeOpinion);
            }
        }
    }
}
