using System.Collections.Generic;

namespace TDRS.StatSystem
{
    public class Stat
    {
        #region Attribute
        protected float _baseValue;
        protected float _value;
        protected List<StatModifier> _modifiers;
        protected float _minValue;
        protected float _maxValue;
        protected bool _isDiscrete;
        protected bool _isDirty;
        #endregion

        #region Properties
        public float BaseValue
        {
            get
            {
                return _baseValue;
            }
            set
            {
                _baseValue = value;
                _isDirty = true;
            }
        }

        public float Value
        {
            get
            {
                if (_isDirty)
                    RecalculateValue();
                return _value;
            }
        }

        public float MinValue => _minValue;

        public float MaxValue => _maxValue;

        public bool IsDiscrete => _isDiscrete;

        public float Normalized
        {
            get
            {

                return (Value - MinValue) / (MaxValue - MinValue);
            }
        }
        #endregion

        #region Constructors
        public Stat(
            float baseValue,
            float minValue = -999999f,
            float maxValue = 999999f,
            bool isDiscrete = false
        )
        {
            _baseValue = baseValue;
            _value = baseValue;
            _modifiers = new List<StatModifier>();
            _isDirty = false;
            _isDiscrete = isDiscrete;
            _minValue = minValue;
            _maxValue = maxValue;
        }
        #endregion

        #region Methods
        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public bool RemoveModifier(StatModifier modifier)
        {
            return false;
        }

        public bool RemoveModifiersFromSource(object source)
        {
            return false;
        }

        protected void RecalculateValue()
        {
            return;
        }
        #endregion
    }
}
