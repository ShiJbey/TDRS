namespace TDRS
{
	/// <summary>
	/// Configuration information used to create relationship instances.
	/// </summary>
	public class RelationshipSchema
	{
		/// <summary>
		/// The agent type of the owner of the relationship.
		/// </summary>
		public string OwnerType { get; }

		/// <summary>
		/// The agent type of the target of the relationship.
		/// </summary>
		public string TargetType { get; }

		/// <summary>
		/// The stats associated with this agent type.
		/// </summary>
		public StatSchema[] Stats { get; }

		/// <summary>
		/// Default traits added to this agent type when created.
		/// </summary>
		public string[] Traits { get; }

		public RelationshipSchema(string ownerType, string targetType, StatSchema[] stats, string[] traits)
		{
			OwnerType = ownerType;
			TargetType = targetType;
			Stats = stats;
			Traits = traits;
		}
	}
}
