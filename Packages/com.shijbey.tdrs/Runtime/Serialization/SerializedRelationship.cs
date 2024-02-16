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
		public List<SerializedTraitInstance> traits { get; set; }
		public List<SerializedStat> stats { get; set; }
		public List<string> activeSocialRules { get; set; }

		public SerializedRelationship()
		{
			owner = "";
			target = "";
			traits = new List<SerializedTraitInstance>();
			stats = new List<SerializedStat>();
			activeSocialRules = new List<string>();
		}
	}
}
