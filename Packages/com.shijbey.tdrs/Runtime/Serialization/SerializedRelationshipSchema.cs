namespace TDRS.Serialization
{
	public class SerializedRelationshipSchema
	{
		public string ownerType { get; set; }

		public string targetType { get; set; }

		public SerializedStatSchema[] stats { get; set; }

		public string[] traits { get; set; }

		public SerializedRelationshipSchema()
		{
			ownerType = "";
			targetType = "";
			stats = new SerializedStatSchema[0];
			traits = new string[0];
		}
	}
}
