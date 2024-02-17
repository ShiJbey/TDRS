namespace TDRS
{
	/// <summary>
	/// An agent or relationship in the social engine.
	/// </summary>
	public interface ISocialEntity
	{
		/// <summary>
		/// A reference to the manager that owns this entity.
		/// </summary>
		public SocialEngine Engine { get; }

		/// <summary>
		/// The collection of traits associated with this entity.
		/// </summary>
		public TraitManager Traits { get; }

		/// <summary>
		/// A collection of stats associated with this entity.
		/// </summary>
		public StatManager Stats { get; }
	}
}
