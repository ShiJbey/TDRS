using System.Collections.Generic;
using UnityEngine;
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
		/// A GameObject associated with this entity
		/// </summary>
		public GameObject GameObject { get; set; }

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
			GameObject = null;
			Traits = new Traits();
			Stats = new Dictionary<string, Stat>();
			SocialRules = new SocialRules();
		}

		public SocialEntity(
			TDRSManager manager,
			string entityID,
			GameObject gameObject
		)
		{
			Manager = manager;
			EntityID = entityID;
			GameObject = gameObject;
			Traits = new Traits();
			Stats = new Dictionary<string, Stat>();
			SocialRules = new SocialRules();
		}

		#endregion

		#region Methods
		public abstract void OnTraitAdded(Trait trait);

		public abstract void OnTraitRemoved(Trait trait);

		public abstract void OnSocialRuleAdded(SocialRule rule);

		public abstract void OnSocialRuleRemoved(SocialRule rule);
		#endregion
	}
}
