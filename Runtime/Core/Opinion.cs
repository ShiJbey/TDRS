using System;
using System.Collections.Generic;

namespace TraitBasedOpinionSystem.Core
{
    public class Opinion
    {
        public static readonly float OPINION_MAX = 100;
        public static readonly float OPINION_MIN = -100;

        protected float _baseValue;
        protected bool _isDirty = true;
        protected int _value = 0;
        protected readonly Actor _subject;
        protected readonly Actor _target;
        protected readonly List<OpinionModifier> _opinionModifiers;

        public Opinion(Actor subject, Actor target, float baseValue = 0)
        {
            _subject = subject;
            _target = target;
            _baseValue = baseValue;
            _opinionModifiers = new List<OpinionModifier>();
        }

        public int Value
        {
            get
            {
                if (_isDirty)
                {
                    _value = CalculateValue();
                    _isDirty = false;
                }
                return _value;
            }
        }

        public Actor Subject { get { return _subject; } }

        public Actor Target { get { return _target; } }

        public void AddModifier(OpinionModifier modifier)
        {
            _isDirty = true;
            _opinionModifiers.Add(modifier);
        }

        public bool RemoveModifier(OpinionModifier modifier)
        {
            _isDirty = true;
            return _opinionModifiers.Remove(modifier);
        }

        private int CalculateValue()
        {
            float finalValue = _baseValue;
            foreach (var trait in _subject.GetTraits())
            {
                foreach (var modifier in trait.GetModifiers())
                {
                    if (modifier.Precondition != null && modifier.Precondition(_subject, _target, this))
                    {
                        finalValue += modifier.Value;
                    }
                }
            }
            return (int)Math.Round(Math.Clamp(finalValue, OPINION_MIN, OPINION_MAX));
        }
    }
}
