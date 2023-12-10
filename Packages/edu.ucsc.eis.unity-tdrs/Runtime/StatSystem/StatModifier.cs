namespace TDRS.StatSystem
{
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

		#endregion

		#region Constructors

		public StatModifier(
			string stat,
			string description,
			float value,
			StatModifierType modifierType,
			object source = null
		)
		{
			Stat = stat;
			Description = description;
			Value = value;
			ModifierType = modifierType;
			Order = (int)modifierType;
			Source = source;
		}

		public StatModifier(
			string stat,
			string description,
			float value,
			StatModifierType modifierType,
			int order,
			object source = null
		)
		{
			Stat = stat;
			Description = description;
			Value = value;
			ModifierType = modifierType;
			Order = order;
			Source = source;
		}

		#endregion
	}
}
