using System.Collections.Generic;
using UnityEngine;
using TDRS.StatSystem;


namespace TDRS
{
	public class TDRSRelationship : SocialEntity
	{
		#region Properties

		/// <summary>
		/// The entity that owns the relationship
		/// </summary>
		public SocialEntity Owner { get; }

		/// <summary>
		/// The entity the relationship is directed toward
		/// </summary>
		public SocialEntity Target { get; }

		#endregion

		#region Constructors

		public TDRSRelationship(
			TDRSManager manager,
			string entityID,
			SocialEntity owner,
			SocialEntity target
		) : base(manager, entityID)
		{
			Owner = owner;
			Target = target;
		}

		public TDRSRelationship(
			TDRSManager manager,
			string entityID,
			SocialEntity owner,
			SocialEntity target,
			GameObject gameObject
		) : base(manager, entityID, gameObject)
		{
			Owner = owner;
			Target = target;
		}

		#endregion

		#region Methods

		public override void OnTraitAdded(Trait trait)
		{
			throw new System.NotImplementedException();
		}

		public override void OnTraitRemoved(Trait trait)
		{
			throw new System.NotImplementedException();
		}

		public override void OnSocialRuleAdded(SocialRule rule)
		{
			throw new System.NotImplementedException();
		}

		public override void OnSocialRuleRemoved(SocialRule rule)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}
