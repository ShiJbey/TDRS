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

		public AgentConfig()
		{
			agentType = "";
			stats = new StatSchema[0];
			traits = new string[0];
		}

		public AgentConfig(string agentType, StatSchema[] stats, string[] traits)
		{
			this.agentType = agentType;
			this.stats = stats;
			this.traits = traits;
		}
	}
}
