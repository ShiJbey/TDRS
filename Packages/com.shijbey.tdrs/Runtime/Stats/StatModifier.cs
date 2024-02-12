namespace TDRS
{
	/// <summary>
	/// A modifier applied to a agent or relationship that changes their stat values
	/// </summary>
	public class StatModifier
	{
		#region Properties

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

		#endregion

		#region Constructors

		public StatModifier(
			float value,
			StatModifierType modifierType,
			object source = null
		)
		{
			Value = value;
			ModifierType = modifierType;
			Order = (int)modifierType;
			Source = source;
		}

		public StatModifier(
			float value,
			StatModifierType modifierType,
			int order,
			object source = null
		)
		{
			Value = value;
			ModifierType = modifierType;
			Order = order;
			Source = source;
		}

		#endregion
	}
}
