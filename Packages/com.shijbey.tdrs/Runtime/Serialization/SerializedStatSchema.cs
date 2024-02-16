namespace TDRS.Serialization
{
	public class SerializedStatSchema
	{
		public string statName { get; set; }
		public float baseValue { get; set; }
		public float maxValue { get; set; }
		public float minValue { get; set; }
		public bool isDiscrete { get; set; }

		public SerializedStatSchema()
		{
			this.statName = "";
			this.baseValue = 0;
			this.maxValue = 0;
			this.minValue = 0;
			this.isDiscrete = true;
		}
	}
}
