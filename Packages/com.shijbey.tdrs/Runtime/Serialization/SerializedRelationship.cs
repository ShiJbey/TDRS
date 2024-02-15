using System.Collections.Generic;

namespace TDRS.Serialization
{
	/// <summary>
	/// Serialized data about a relationship instance.
	/// </summary>
	public class SerializedRelationship
	{
		public string owner;
		public string target;
		public List<string> traits;
		public List<SerializedStat> stats;

		public SerializedRelationship()
		{
			traits = new List<string>();
			stats = new List<SerializedStat>();
		}
	}
}
