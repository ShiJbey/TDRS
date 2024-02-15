using System.Collections.Generic;

namespace TDRS.Serialization
{
	/// <summary>
	/// Serialized data about an agent.
	/// </summary>
	public class SerializedAgent
	{
		public string uid;
		public string agentType;
		public List<string> traits;
		public List<SerializedStat> stats;

		public SerializedAgent()
		{
			traits = new List<string>();
			stats = new List<SerializedStat>();
		}
	}
}
