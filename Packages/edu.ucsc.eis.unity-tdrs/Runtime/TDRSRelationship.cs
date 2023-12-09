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
		public TDRSNode Owner { get; }

		/// <summary>
		/// The entity the relationship is directed toward
		/// </summary>
		public TDRSNode Target { get; }

		#endregion

		#region Constructors

		public TDRSRelationship(
			TDRSManager manager,
			string entityID,
			TDRSNode owner,
			TDRSNode target
		) : base(manager, entityID)
		{
			Owner = owner;
			Target = target;
		}

		#endregion

		#region Methods

		public override void OnTraitAdded(Trait trait)
		{
			return;
		}

		public override void OnTraitRemoved(Trait trait)
		{
			return;
		}

		public override void OnSocialRuleAdded(SocialRule rule)
		{
			return;
		}

		public override void OnSocialRuleRemoved(SocialRule rule)
		{
			return;
		}

		#endregion
	}
}
