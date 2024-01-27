using System;

namespace TDRS
{
	/// <summary>
	/// Configuration information used to create relationship instances.
	/// </summary>
	[Serializable]
	public class RelationshipConfig
	{
		/// <summary>
		/// The agent type of the owner of the relationship.
		/// </summary>
		public string ownerAgentType;

		/// <summary>
		/// The agent type of the target of the relationship.
		/// </summary>
		public string targetAgentType;

		/// <summary>
		/// The stats associated with this agent type.
		/// </summary>
		public StatSchema[] stats;

		/// <summary>
		/// Default traits added to this agent type when created.
		/// </summary>
		public string[] traits;
	}
}
