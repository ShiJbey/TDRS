namespace TDRS
{
	/// <summary>
	/// Configuration settings used to create agents.
	/// </summary>
	public class AgentSchema
	{
		/// <summary>
		/// The type of agent this creates.
		/// </summary>
		public string AgentType { get; }

		/// <summary>
		/// The stats associated with this agent type.
		/// </summary>
		public StatSchema[] Stats { get; }

		/// <summary>
		/// Default traits added to this agent type when created.
		/// </summary>
		public string[] Traits { get; }

		public AgentSchema(string agentType, StatSchema[] stats, string[] traits)
		{
			AgentType = agentType;
			Stats = stats;
			Traits = traits;
		}
	}
}
