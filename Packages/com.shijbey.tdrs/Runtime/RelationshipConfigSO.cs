using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// Configuration information used to create relationship instances.
	/// </summary>
	[CreateAssetMenu(menuName = "TDRS/Relationship Config")]
	public class RelationshipConfigSO : ScriptableObject
	{
		/// <summary>
		/// The agent type of the owner of the relationship.
		/// </summary>
		public AgentConfigSO ownerAgentType;

		/// <summary>
		/// The agent type of the target of the relationship.
		/// </summary>
		public AgentConfigSO targetAgentType;

		/// <summary>
		/// The stats associated with this agent type.
		/// </summary>
		public StatSchema[] stats;

		/// <summary>
		/// Default traits added to this agent type when created.
		/// </summary>
		public string[] traits;

		/// <summary>
		/// Create relationship config from scriptable object
		/// </summary>
		/// <returns></returns>
		public RelationshipConfig CreateRelationshipConfig()
		{
			return new RelationshipConfig()
			{
				ownerAgentType = ownerAgentType.agentType,
				targetAgentType = targetAgentType.agentType,
				stats = stats,
				traits = traits,
			};
		}
	}
}
