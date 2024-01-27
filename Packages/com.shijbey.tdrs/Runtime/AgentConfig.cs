using System;

namespace TDRS
{
	/// <summary>
	/// Configuration settings used to create agents.
	/// </summary>
	[Serializable]
	public class AgentConfig
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
	}
}
