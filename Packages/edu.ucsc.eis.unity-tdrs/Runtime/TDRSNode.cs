using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// A vertex/node within the social graph. This might represent a character, faction, concept,
	/// or any entity that characters might have a relationship toward.
	/// </summary>
	public class TDRSNode : SocialEntity
	{
		#region Properties

		public string NodeType { get; }

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
			string nodeType,
			string entityID
		) : base(engine, entityID)
		{
			NodeType = nodeType;
			Traits = new Traits();
			SocialRules = new SocialRules();
			IncomingRelationships = new Dictionary<TDRSNode, TDRSRelationship>();
			OutgoingRelationships = new Dictionary<TDRSNode, TDRSRelationship>();
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
