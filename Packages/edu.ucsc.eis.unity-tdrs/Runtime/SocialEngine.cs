using System.Collections.Generic;
using RePraxis;

namespace TDRS
{
	/// <summary>
	/// The main interface for managing the relationship system.
	/// </summary>
	public class SocialEngine
	{
		#region Protected Fields

		protected Dictionary<string, TDRSNode> _nodes;

		protected Dictionary<(string, string), TDRSRelationship> _relationships;

		#endregion

		#region Public Properties

		public TraitLibrary TraitLibrary { get; }
		public EffectLibrary EffectLibrary { get; }
		public PreconditionLibrary PreconditionLibrary { get; }
		public IEnumerable<TDRSNode> Nodes => _nodes.Values;
		public RePraxisDatabase DB { get; }

		#endregion

		#region Constructors

		public SocialEngine()
		{
			TraitLibrary = new TraitLibrary();
			EffectLibrary = new EffectLibrary();
			PreconditionLibrary = new PreconditionLibrary();
			DB = new RePraxisDatabase();
			_nodes = new Dictionary<string, TDRSNode>();
			_relationships = new Dictionary<(string, string), TDRSRelationship>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Retrieves the social entity with the given ID or creates one
		/// if one is not found.
		/// </summary>
		/// <param name="nodeId"></param>
		public void AddNode(TDRSNode node)
		{

			if (_nodes.ContainsKey(node.UID))
			{
				throw new System.Exception($"Entity already exists with ID: {node.UID}");
			}

			_nodes[node.UID] = node;

			DB.Insert($"{node.UID}");
		}

		/// <summary>
		/// Add a new relationship
		/// </summary>
		/// <param name="relationship">
		public void AddRelationship(TDRSRelationship relationship)
		{
			if (_relationships.ContainsKey((relationship.Owner.UID, relationship.Target.UID)))
			{
				throw new System.Exception(
					"A relationship already exists between "
					+ $"{relationship.Owner} and {relationship.Target}.");
			}

			_relationships[(relationship.Owner.UID, relationship.Target.UID)] = relationship;

			var owner = relationship.Owner;
			var target = relationship.Target;

			owner.OutgoingRelationships[target] = relationship;
			target.IncomingRelationships[owner] = relationship;


			// Apply outgoing social rules from the owner
			foreach (var rule in owner.SocialRules.Rules)
			{
				if (rule.IsOutgoing && rule.CheckPreconditions(relationship))
				{
					rule.OnAdd(relationship);
					relationship.SocialRules.AddSocialRule(rule);
				}
			}

			// Apply incoming social rules from the target
			foreach (var rule in target.SocialRules.Rules)
			{
				if (!rule.IsOutgoing && rule.CheckPreconditions(relationship))
				{
					rule.OnAdd(relationship);
					relationship.SocialRules.AddSocialRule(rule);
				}
			}

			DB.Insert($"{owner.UID}.relationships.{target.UID}");
		}

		/// <summary>
		/// Get a reference to a node.
		/// </summary>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException">If no node found with given ID.</exception>
		public TDRSNode GetNode(string nodeId)
		{
			if (!_nodes.ContainsKey(nodeId))
			{
				throw new KeyNotFoundException($"Cannot find node with ID: {nodeId}.");
			}

			return _nodes[nodeId];
		}

		/// <summary>
		/// Get a reference to a relationship.
		/// </summary>
		/// <param name="ownerId"></param>
		/// <param name="targetId"></param>
		/// <returns></returns>
		public TDRSRelationship GetRelationship(string ownerId, string targetId)
		{
			if (!_nodes.ContainsKey(ownerId))
			{
				throw new KeyNotFoundException($"Cannot find node with ID: {ownerId}.");
			}

			if (!_nodes.ContainsKey(targetId))
			{
				throw new KeyNotFoundException($"Cannot find node with ID: {targetId}.");
			}

			var owner = GetNode(ownerId);
			var target = GetNode(targetId);

			if (!owner.OutgoingRelationships.ContainsKey(target))
			{
				throw new KeyNotFoundException(
					$"Cannot find relationship from {ownerId} to {targetId}.");
			}

			return owner.OutgoingRelationships[target];
		}

		/// <summary>
		/// Check if a node exists
		/// </summary>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		public bool HasNode(string nodeId)
		{
			return _nodes.ContainsKey(nodeId);
		}

		/// <summary>
		/// Check if a relationship exists
		/// </summary>
		/// <param name="ownerId"></param>
		/// <param name="targetId"></param>
		/// <returns></returns>
		public bool HasRelationship(string ownerId, string targetId)
		{
			if (!_nodes.ContainsKey(ownerId)) return false;
			if (!_nodes.ContainsKey(targetId)) return false;

			var targetNode = _nodes[targetId];

			return _nodes[ownerId].OutgoingRelationships.ContainsKey(targetNode);
		}

		/// <summary>
		/// Try to get a reference to a node
		/// </summary>
		/// <param name="nodeId"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public bool TryGetNode(string nodeId, out TDRSNode node)
		{
			node = null;

			if (!_nodes.ContainsKey(nodeId)) return false;

			node = _nodes[nodeId];
			return true;
		}

		/// <summary>
		/// Try to get a reference to a relationship
		/// </summary>
		/// <param name="ownerId"></param>
		/// <param name="targetId"></param>
		/// <param name="relationship"></param>
		/// <returns></returns>
		public bool TryGetRelationship(
			string ownerId,
			string targetId,
			out TDRSRelationship relationship)
		{
			relationship = null;

			if (!_nodes.ContainsKey(ownerId)) return false;
			if (!_nodes.ContainsKey(targetId)) return false;

			var ownerNode = _nodes[ownerId];
			var targetNode = _nodes[targetId];

			if (!ownerNode.OutgoingRelationships.ContainsKey(targetNode)) return false;

			relationship = _nodes[ownerId].OutgoingRelationships[targetNode];
			return true;
		}

		#endregion
	}
}
