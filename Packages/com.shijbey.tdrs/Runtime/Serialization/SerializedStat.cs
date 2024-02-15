using System.Collections.Generic;

namespace TDRS.Serialization
{
	public class SerializedStat
	{
		public string Name { get; set; }

		public float BaseValue { get; set; }

		public List<SerializedModifier> Modifiers { get; set; }

		public SerializedStat()
		{
			Name = "";
			BaseValue = 0f;
			Modifiers = new List<SerializedModifier>();
		}
	}
}
