using System.Collections.Generic;

namespace TDRS
{
	public class TDRSNodeSocialRules : SocialRules
	{
		/// <summary>
		/// Reference to the node these rules manage
		/// </summary>
		protected TDRSNode _node;

		public TDRSNodeSocialRules(TDRSNode node) : base()
		{
			_node = node;
		}

		public override void AddSocialRule(SocialRule rule)
		{
			base.AddSocialRule(rule);

			Dictionary<TDRSNode, TDRSRelationship> relationships;

			if (rule.IsOutgoing)
			{
				relationships = _node.OutgoingRelationships;
			}
			else
			{
				relationships = _node.IncomingRelationships;
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
				relationships = _node.OutgoingRelationships;
			}
			else
			{
				relationships = _node.IncomingRelationships;
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
	}
}
