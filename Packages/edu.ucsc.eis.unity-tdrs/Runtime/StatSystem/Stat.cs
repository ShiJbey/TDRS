using System;
using System.Collections.Generic;

namespace TDRS.StatSystem
{
	/// <summary>
	/// Tracks a single stat about an entity
	/// </summary>
	public class Stat
	{
		#region Attribute

		/// <summary>
		/// The value of the stat without modifiers
		/// </summary>
		protected float _baseValue;

		/// <summary>
		///  The last calculated value of the stat including modifiers
		/// </summary>
		protected float _value;

		/// <summary>
		/// Modifiers currently attached to this stat
		/// </summary>
		protected List<StatModifier> _modifiers;

		/// <summary>
		/// The minimum value of the stat
		/// </summary>
		protected float _minValue;

		/// <summary>
		/// The maximum value of the stat
		/// </summary>
		protected float _maxValue;

		/// <summary>
		/// Should the calculated value be truncated to an integer
		/// </summary>
		protected bool _isDiscrete;

		/// <summary>
		/// Have there been any changes that require the value to be recalculated
		/// </summary>
		protected bool _isDirty;

		#endregion

		#region Properties

		/// <summary>
		/// Get the base value of the stat without modifiers
		/// </summary>
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

		/// <summary>
		/// Get the current value of the stat including modifiers
		/// </summary>
		public float Value
		{
			get
			{
				if (_isDirty)
					RecalculateValue();
				return _value;
			}
		}

		/// <summary>
		/// Get the minimum value of the stat
		/// </summary>
		public float MinValue => _minValue;

		/// <summary>
		/// Get the maximum value of the stat
		/// </summary>
		public float MaxValue => _maxValue;

		/// <summary>
		/// Returns true if the final value should be floored to an int
		/// </summary>
		public bool IsDiscrete => _isDiscrete;

		/// <summary>
		/// Get the value of the stat normalized on the interval from
		/// its minimum to maximum value
		/// </summary>
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

		/// <summary>
		/// Add the given modifier to the stat
		/// </summary>
		/// <param name="modifier"></param>
		public void AddModifier(StatModifier modifier)
		{
			_modifiers.Add(modifier);
			_modifiers.Sort((a, b) =>
			{
				if (a.Order < b.Order)
				{
					return -1;
				}

				if (a.Order == b.Order)
				{
					return 0;
				}

				return 1;
			});
			_isDirty = true;
		}

		/// <summary>
		/// Remove the given modifier from the stat.
		/// </summary>
		/// <param name="modifier"></param>
		/// <returns></returns>
		public bool RemoveModifier(StatModifier modifier)
		{
			var success = _modifiers.Remove(modifier);

			if (success)
			{
				_isDirty = true;
			}

			return success;
		}

		/// <summary>
		/// Remove all modifiers that come from the given source
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public bool RemoveModifiersFromSource(object source)
		{
			var removed_any_modifier = false;

			for (int i = _modifiers.Count; i >= 0; i--)
			{
				var modifier = _modifiers[i];
				if (modifier.Source == source)
				{
					_modifiers.RemoveAt(i);
					removed_any_modifier = true;
					_isDirty = true;
				}
			}

			return removed_any_modifier;
		}

		/// <summary>
		/// Recalculate the final value of this stat, taking into account its
		/// modifiers and max/min values.
		/// </summary>
		protected void RecalculateValue()
		{
			float finalValue = _baseValue;
			float percentAddSum = 0f;

			for (int i = 0; i < _modifiers.Count; i++)
			{
				var modifier = _modifiers[i];

				if (modifier.ModifierType == StatModifierType.FLAT)
				{
					finalValue += modifier.Value;
				}

				if (modifier.ModifierType == StatModifierType.PERCENT_ADD)
				{
					percentAddSum += modifier.Value;

					if (
						(i + 1) >= _modifiers.Count
						|| _modifiers[i + 1].ModifierType != StatModifierType.PERCENT_ADD
					)
					{
						finalValue = finalValue * (1 + percentAddSum);
						percentAddSum = 0f;
					}
				}

				if (modifier.ModifierType == StatModifierType.PERCENT_MULTIPLY)
				{
					finalValue = finalValue * (1 + modifier.Value);
				}
			}

			finalValue = Math.Max(MinValue, Math.Min(MaxValue, finalValue));

			if (_isDiscrete)
			{
				finalValue = (float)Math.Floor(finalValue);
			}

			_value = finalValue;
			_isDirty = false;
		}

		#endregion
	}
}
