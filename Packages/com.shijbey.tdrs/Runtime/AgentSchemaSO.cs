using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// Configuration settings used to create agents.
	/// </summary>
	[CreateAssetMenu(menuName = "TDRS/Agent Schema")]
	public class AgentSchemaSO : ScriptableObject
	{
		/// <summary>
		/// The type of agent this creates.
		/// </summary>
		public string agentType;

		/// <summary>
		/// The stats associated with this agent type.
		/// </summary>
		public StatSchema[] stats;

		/// <summary>
		/// Default traits added to this agent type when created.
		/// </summary>
		public string[] traits;

		/// <summary>
		/// Create an agent schema from the scriptable object data.
		/// </summary>
		/// <returns></returns>
		public AgentSchema CreateAgentSchema()
		{
			return new AgentSchema(
				agentType, stats, traits
			);
		}
	}
}
