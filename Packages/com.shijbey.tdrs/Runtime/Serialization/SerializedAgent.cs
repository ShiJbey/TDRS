using System.Collections.Generic;

namespace TDRS.Serialization
{
	/// <summary>
	/// Serialized data about an agent.
	/// </summary>
	public class SerializedAgent
	{
		public string uid { get; set; }
		public string agentType { get; set; }
		public List<SerializedTraitInstance> traits { get; set; }
		public List<SerializedStat> stats { get; set; }

		public SerializedAgent()
		{
			uid = "";
			agentType = "Agent";
			traits = new List<SerializedTraitInstance>();
			stats = new List<SerializedStat>();
		}
	}
}
