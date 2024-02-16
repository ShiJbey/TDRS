namespace TDRS
{
	/// <summary>
	/// Data used to instantiate a <c>StatModifier</c>
	/// </summary>
	public class StatModifierData
	{
		/// <summary>
		/// The name of the stat to modify
		/// </summary>
		public string StatName { get; }

		/// <summary>
		/// The modifier value to apply.
		/// </summary>
		public float Value { get; }

		/// <summary>
		/// How to mathematically apply the modifier value.
		/// </summary>
		public StatModifierType ModifierType { get; }

		public StatModifierData(string statName, float value, StatModifierType modifierType)
		{
			StatName = statName;
			Value = value;
			ModifierType = modifierType;
		}
	}
}
