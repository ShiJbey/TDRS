namespace TDRS.Serialization
{
	public class SerializedStatModifierData
	{

		/// <summary>
		/// The name of the stat to modify
		/// </summary>
		public string statName { get; set; }

		/// <summary>
		/// The modifier value to apply.
		/// </summary>
		public float value { get; set; }

		/// <summary>
		/// How to mathematically apply the modifier value.
		/// </summary>
		public string modifierType { get; set; }

		public SerializedStatModifierData()
		{
			statName = "";
			value = 0;
			modifierType = "FLAT";
		}
	}
}
