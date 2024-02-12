using System;
using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// Tracks a single stat about an agent or relationship.
	/// <para>
	/// This class and its associated classes that make up the state system were adapted from
	/// the Unity Stat System tutorial series by: Kryzarel.
	/// https://www.youtube.com/watch?v=SH25f3cXBVc
	/// </para>
	/// </summary>
	public class Stat
	{
		#region Constants

		/// <summary>
		/// Number of fractional digits to round to when calculating the value or normalized value
		/// of a stat.
		/// </summary>
		public const int ROUND_PRECISION = 3;

		#endregion

		#region Fields

		/// <summary>
		/// The value of the stat without modifiers
		/// </summary>
		protected float m_baseValue;

		/// <summary>
		///  The last calculated value of the stat including modifiers
		/// </summary>
		protected float m_value;

		/// <summary>
		/// The value of this stat normalized from on the interval from the minimum
		/// value to the max value.
		/// </summary>
		protected float m_normalizedValue;

		/// <summary>
		/// Modifiers currently attached to this stat
		/// </summary>
		protected List<StatModifier> m_modifiers;

		/// <summary>
		/// The minimum value of the stat
		/// </summary>
		protected float m_minValue;

		/// <summary>
		/// The maximum value of the stat
		/// </summary>
		protected float m_maxValue;

		/// <summary>
		/// Should the calculated value be truncated to an integer
		/// </summary>
		protected bool m_isDiscrete;

		/// <summary>
		/// Have there been any changes that require the value to be recalculated
		/// </summary>
		protected bool m_isDirty;

		#endregion

		#region Properties

		/// <summary>
		/// Get the base value of the stat without modifiers
		/// </summary>
		public float BaseValue
		{
			get
			{
				return m_baseValue;
			}
			set
			{
				m_baseValue = value;
				m_isDirty = true;
				OnValueChanged?.Invoke(this, new OnValueChangedArgs(Value));
			}
		}

		/// <summary>
		/// Get the current value of the stat including modifiers
		/// </summary>
		public float Value
		{
			get
			{
				if (m_isDirty)
					RecalculateValue();
				return m_value;
			}
		}

		/// <summary>
		/// Get the minimum value of the stat
		/// </summary>
		public float MinValue => m_minValue;

		/// <summary>
		/// Get the maximum value of the stat
		/// </summary>
		public float MaxValue => m_maxValue;

		/// <summary>
		/// Returns true if the final value should be floored to an int
		/// </summary>
		public bool IsDiscrete => m_isDiscrete;

		/// <summary>
		/// Get the value of the stat normalized on the interval from
		/// its minimum to maximum value
		/// </summary>
		public float Normalized
		{
			get
			{
				if (m_isDirty)
					RecalculateValue();
				return m_normalizedValue;
			}
		}

		/// <summary>
		/// All the modifiers affecting the stat.
		/// </summary>
		public IList<StatModifier> Modifiers => m_modifiers;

		#endregion

		#region Events

		/// <summary>
		/// Event invoked whenever something happens that changes the value of the stat.
		/// </summary>
		public event EventHandler<OnValueChangedArgs> OnValueChanged;
		public class OnValueChangedArgs : EventArgs
		{
			public float Value { get; }

			public OnValueChangedArgs(float value)
			{
				Value = value;
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
			m_baseValue = baseValue;
			m_value = baseValue;
			m_modifiers = new List<StatModifier>();
			m_isDirty = true;
			m_isDiscrete = isDiscrete;
			m_minValue = minValue;
			m_maxValue = maxValue;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add the given modifier to the stat
		/// </summary>
		/// <param name="modifier"></param>
		public void AddModifier(StatModifier modifier)
		{
			m_modifiers.Add(modifier);
			m_modifiers.Sort((a, b) =>
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
			m_isDirty = true;
			OnValueChanged?.Invoke(this, new OnValueChangedArgs(Value));
		}

		/// <summary>
		/// Remove the given modifier from the stat.
		/// </summary>
		/// <param name="modifier"></param>
		/// <returns></returns>
		public bool RemoveModifier(StatModifier modifier)
		{
			var success = m_modifiers.Remove(modifier);

			if (success)
			{
				m_isDirty = true;
				OnValueChanged?.Invoke(this, new OnValueChangedArgs(Value));
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

			for (int i = m_modifiers.Count - 1; i >= 0; i--)
			{
				var modifier = m_modifiers[i];
				if (modifier.Source == source)
				{
					m_modifiers.RemoveAt(i);
					removed_any_modifier = true;
					m_isDirty = true;
				}
			}

			if (removed_any_modifier && OnValueChanged != null)
			{
				OnValueChanged?.Invoke(this, new OnValueChangedArgs(Value));
			}

			return removed_any_modifier;
		}

		/// <summary>
		/// Recalculate the final value of this stat, taking into account its
		/// modifiers and max/min values.
		/// </summary>
		protected void RecalculateValue()
		{
			float finalValue = m_baseValue;
			float percentAddSum = 0f;

			for (int i = 0; i < m_modifiers.Count; i++)
			{
				var modifier = m_modifiers[i];

				if (modifier.ModifierType == StatModifierType.FLAT)
				{
					finalValue += modifier.Value;
				}

				if (modifier.ModifierType == StatModifierType.PERCENT_ADD)
				{
					percentAddSum += modifier.Value;

					if (
						(i + 1) >= m_modifiers.Count
						|| m_modifiers[i + 1].ModifierType != StatModifierType.PERCENT_ADD
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

			if (m_isDiscrete)
			{
				finalValue = (float)Math.Floor(finalValue);
			}

			finalValue = (float)Math.Round(finalValue, ROUND_PRECISION);

			m_normalizedValue = (float)Math.Round(
				(finalValue - MinValue) / (MaxValue - MinValue),
				ROUND_PRECISION
			);

			m_value = finalValue;
			m_isDirty = false;
		}

		#endregion
	}
}
