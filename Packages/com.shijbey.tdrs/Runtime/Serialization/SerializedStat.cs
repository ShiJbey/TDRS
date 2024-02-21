namespace TDRS.Serialization
{
	public class SerializedStat
	{
		public string name { get; set; }

		public float baseValue { get; set; }

		public SerializedStat()
		{
			name = "";
			baseValue = 0f;
		}
	}
}
