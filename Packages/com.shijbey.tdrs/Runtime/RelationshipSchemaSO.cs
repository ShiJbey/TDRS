using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// Configuration information used to create relationship instances.
	/// </summary>
	[CreateAssetMenu(menuName = "TDRS/Relationship Schema")]
	public class RelationshipSchemaSO : ScriptableObject
	{
		/// <summary>
		/// The agent type of the owner of the relationship.
		/// </summary>
		public AgentSchemaSO ownerType;

		/// <summary>
		/// The agent type of the target of the relationship.
		/// </summary>
		public AgentSchemaSO targetType;

		/// <summary>
		/// The stats associated with this agent type.
		/// </summary>
		public StatSchema[] stats;

		/// <summary>
		/// Default traits added to this agent type when created.
		/// </summary>
		public string[] traits;

		/// <summary>
		/// Create relationship schema from scriptable object
		/// </summary>
		/// <returns></returns>
		public RelationshipSchema CreateRelationshipSchema()
		{
			return new RelationshipSchema(
				ownerType.agentType,
				targetType.agentType,
				stats,
				traits
			);
		}
	}
}
