namespace TDRS
{
	[System.Serializable]
	public class StatSchema
	{
		public string statName;
		public float baseValue;
		public float maxValue;
		public float minValue;
		public bool isDiscrete;

		public StatSchema(
			string statName,
			float baseValue,
			float maxValue,
			float minValue,
			bool isDiscrete)
		{
			this.statName = statName;
			this.baseValue = baseValue;
			this.maxValue = maxValue;
			this.minValue = minValue;
			this.isDiscrete = isDiscrete;
		}
	}
}
