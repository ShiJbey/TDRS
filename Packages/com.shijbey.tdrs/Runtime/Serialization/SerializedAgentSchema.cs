namespace TDRS.Serialization
{
	public class SerializedAgentSchema
	{
		public string agentType { get; set; }

		public SerializedStatSchema[] stats { get; set; }

		public string[] traits { get; set; }

		public SerializedAgentSchema()
		{
			agentType = "";
			stats = new SerializedStatSchema[0];
			traits = new string[0];
		}
	}
}
