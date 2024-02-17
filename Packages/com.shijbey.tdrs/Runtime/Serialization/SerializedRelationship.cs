using System.Collections.Generic;

namespace TDRS.Serialization
{
	/// <summary>
	/// Serialized data about a relationship instance.
	/// </summary>
	public class SerializedRelationship
	{
		public string owner { get; set; }
		public string target { get; set; }
		public string relationshipType { get; set; }
		public List<SerializedTraitInstance> traits { get; set; }
		public List<SerializedStat> stats { get; set; }

		public SerializedRelationship()
		{
			owner = "";
			target = "";
			relationshipType = "";
			traits = new List<SerializedTraitInstance>();
			stats = new List<SerializedStat>();
		}
	}
}
