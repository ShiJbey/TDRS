using System.Collections.Generic;
using TDRS.StatSystem;

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
		public Traits Traits { get; }

		/// <summary>
		/// Mapping of stat names to instances
		/// </summary>
		public Dictionary<string, Stat> Stats { get; }

		/// <summary>
		/// All social rules affecting this entity
		/// </summary>
		public SocialRules SocialRules { get; }

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
			Stats = new Dictionary<string, Stat>();
			SocialRules = new SocialRules();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Callback method executed when a trait is added to the entity.
		/// </summary>
		/// <param name="trait"></param>
		public abstract void OnTraitAdded(Trait trait);

		/// <summary>
		/// Callback method executed when a trait is removed from the entity.
		/// </summary>
		/// <param name="trait"></param>
		public abstract void OnTraitRemoved(Trait trait);

		/// <summary>
		/// Callback method executed when a social rule is added to the entity.
		/// </summary>
		/// <param name="rule"></param>
		public abstract void OnSocialRuleAdded(SocialRule rule);

		/// <summary>
		/// Callback method executed when a social rule is removed from the entity.
		/// </summary>
		/// <param name="rule"></param>
		public abstract void OnSocialRuleRemoved(SocialRule rule);

		#endregion
	}
}
