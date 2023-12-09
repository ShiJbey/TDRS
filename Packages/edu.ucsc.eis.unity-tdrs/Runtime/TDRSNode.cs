using System.Collections.Generic;
using UnityEngine;

namespace TDRS
{
	public class TDRSNode : SocialEntity
	{
		#region Properties

		/// <summary>
		/// Relationships directed toward this entity
		/// </summary>
		public Dictionary<TDRSNode, TDRSRelationship> IncomingRelationships { get; }

		/// <summary>
		/// Relationships from this entity directed toward other entities
		/// </summary>
		public Dictionary<TDRSNode, TDRSRelationship> OutgoingRelationships { get; }

		#endregion

		#region Constructors

		public TDRSNode(
			TDRSManager manager,
			string entityID
		) : base(manager, entityID)
		{
			IncomingRelationships = new Dictionary<TDRSNode, TDRSRelationship>();
			OutgoingRelationships = new Dictionary<TDRSNode, TDRSRelationship>();
		}

		public TDRSNode(
			TDRSManager manager,
			string entityID,
			GameObject gameObject
		) : base(manager, entityID, gameObject)
		{
			IncomingRelationships = new Dictionary<TDRSNode, TDRSRelationship>();
			OutgoingRelationships = new Dictionary<TDRSNode, TDRSRelationship>();
		}

		#endregion

		#region Methods

		public override void OnTraitAdded(Trait trait)
		{
			if (GameObject != null)
			{
				var tdrsEntity = GameObject.GetComponent<TDRSEntity>();
				if (tdrsEntity != null)
				{
					tdrsEntity.OnTraitAdded.Invoke(trait);
				}
			}
		}

		public override void OnTraitRemoved(Trait trait)
		{
			if (GameObject != null)
			{
				var tdrsEntity = GameObject.GetComponent<TDRSEntity>();
				if (tdrsEntity != null)
				{
					tdrsEntity.OnTraitRemoved.Invoke(trait);
				}
			}
		}

		public override void OnSocialRuleAdded(SocialRule rule)
		{
			Dictionary<TDRSNode, TDRSRelationship> relationships;

			if (rule.IsOutgoing)
			{
				relationships = OutgoingRelationships;
			}
			else
			{
				relationships = IncomingRelationships;
			}

			foreach (var (_, relationship) in relationships)
			{
				if (rule.CheckPreconditions(relationship))
				{
					relationship.SocialRules.AddSocialRule(rule);
					rule.OnAdd(relationship);
				}
			}
		}

		public override void OnSocialRuleRemoved(SocialRule rule)
		{
			Dictionary<TDRSNode, TDRSRelationship> relationships;
			if (rule.IsOutgoing)
			{
				relationships = OutgoingRelationships;
			}
			else
			{
				relationships = IncomingRelationships;
			}

			foreach (var (_, relationship) in relationships)
			{
				if (relationship.SocialRules.HasSocialRule(rule))
				{
					rule.OnRemove(relationship);
					relationship.SocialRules.RemoveSocialRule(rule);
				}
			}
		}

		#endregion
	}
}
