using UnityEngine;

namespace TDRS.StatSystem
{
	/// <summary>
	/// A modifier applied to a agent or relationship that changes their stat values
	/// </summary>
	public class StatModifier
	{
		#region Properties

		/// <summary>
		/// The name of the stat this modifier is for
		/// </summary>
		public string Stat { get; }

		/// <summary>
		/// A short description of why this modifier exists
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// The modifier value to apply.
		/// </summary>
		public float Value { get; }

		/// <summary>
		/// How to mathematically apply the modifier value.
		/// </summary>
		public StatModifierType ModifierType { get; }

		/// <summary>
		/// The priority of this modifier when calculating final stat values.
		/// </summary>
		public int Order { get; }

		/// <summary>
		/// The object responsible for applying the modifier.
		/// </summary>
		public object Source { get; }

		/// <summary>
		/// The remaining amount of time that the modifier lasts for
		/// </summary>
		public int Duration { get; private set; }

		#endregion

		#region Constructors

		public StatModifier(
			string stat,
			string description,
			float value,
			StatModifierType modifierType,
			int duration,
			object source = null
		)
		{
			Stat = stat;
			Description = description;
			Value = value;
			ModifierType = modifierType;
			Order = (int)modifierType;
			Duration = duration;
			Source = source;
		}

		public StatModifier(
			string stat,
			string description,
			float value,
			StatModifierType modifierType,
			int order,
			int duration,
			object source = null
		)
		{
			Stat = stat;
			Description = description;
			Value = value;
			ModifierType = modifierType;
			Order = order;
			Duration = duration;
			Source = source;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Decrement the amount of time remaining for this modifier
		/// </summary>
		public void DecrementDuration()
		{
			Duration -= 1;
		}

		#endregion
	}
}
