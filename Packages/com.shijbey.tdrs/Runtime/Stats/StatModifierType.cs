namespace TDRS
{
	/// <summary>
	/// Specifier for how a stat modifier is used to calculate the final stat value.
	/// </summary>
	public enum StatModifierType
	{
		/// <summary>
		/// Adds a constant value to the base value.
		/// </summary>
		FLAT = 100,

		/// <summary>
		/// Additively stacks percentage increases on a modified stat.
		/// </summary>
		PERCENT_ADD = 200,

		/// <summary>
		/// Multiplicatively stacks percentage increases on a modified stat.
		/// </summary>
		PERCENT_MULTIPLY = 300,
	}
}
