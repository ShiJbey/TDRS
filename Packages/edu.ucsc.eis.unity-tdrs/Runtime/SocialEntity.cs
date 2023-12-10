namespace TDRS
{
	public abstract class SocialEntity
	{
		#region Properties

		/// <summary>
		/// Get the unique ID of the entity
		/// </summary>
		public string EntityID { get; }

		/// <summary>
		/// A reference to the manager that owns this entity
		/// </summary>
		public TDRSManager Manager { get; }

		/// <summary>
		/// The collection of traits associated with this entity
		/// </summary>
		public Traits Traits { get; protected set; }

		/// <summary>
		/// A collection of stats associated with this entity
		/// </summary>
		public Stats Stats { get; protected set; }

		/// <summary>
		/// All social rules affecting this entity
		/// </summary>
		public SocialRules SocialRules { get; protected set; }

		#endregion

		#region Constructors

		public SocialEntity(
			TDRSManager manager,
			string entityID
		)
		{
			Manager = manager;
			EntityID = entityID;
			Traits = new Traits();
			Stats = new Stats();
			SocialRules = new SocialRules();
		}

		#endregion
	}
}
