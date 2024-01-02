using System.Collections.Generic;
using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// A vertex/node within the social graph. This might represent a character, faction, concept,
	/// or any entity that characters might have a relationship toward.
	/// </summary>
	public class TDRSNode : SocialEntity
	{
		#region Properties

		public GameObject GameObject { get; }

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
			SocialEngine engine,
			string entityID,
			GameObject gameObject
		) : base(engine, entityID)
		{
			GameObject = gameObject;
			IncomingRelationships = new Dictionary<TDRSNode, TDRSRelationship>();
			OutgoingRelationships = new Dictionary<TDRSNode, TDRSRelationship>();

			Traits.OnTraitAdded += (traits, traitID) =>
			{
				Engine.DB.Insert($"{UID}.trait.{traitID}");
			};

			Traits.OnTraitRemoved += (traits, traitID) =>
			{
				Engine.DB.Delete($"{UID}.trait.{traitID}");
			};

			Stats.OnValueChanged += (stats, pair) =>
			{
				string statName = pair.Item1;
				float value = pair.Item2;

				Engine.DB.Insert($"{UID}.stat.{statName}!{value}");
			};
		}

		#endregion

		#region Methods

		public override void AddSocialRule(SocialRule rule)
		{
			base.AddSocialRule(rule);

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

		public override void RemoveSocialRule(SocialRule rule)
		{
			base.RemoveSocialRule(rule);

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
