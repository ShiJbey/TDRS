using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// The main interface for managing the relationship system.
	/// </summary>
	public class SocialEngine
	{
		#region Protected Fields

		protected Dictionary<string, TDRSNode> _nodes;
		protected Dictionary<string, NodeSchema> _nodeSchema;
		protected Dictionary<(string, string), RelationshipSchema> _relationshipSchema;

		#endregion

		#region Public Properties

		public TraitLibrary TraitLibrary { get; }
		public EffectLibrary EffectLibrary { get; }
		public PreconditionLibrary PreconditionLibrary { get; }
		public IEnumerable<TDRSNode> Nodes => _nodes.Values;

		#endregion

		#region Constructors

		public SocialEngine()
		{
			_nodes = new Dictionary<string, TDRSNode>();
			_nodeSchema = new Dictionary<string, NodeSchema>();
			_relationshipSchema = new Dictionary<(string, string), RelationshipSchema>();
			TraitLibrary = new TraitLibrary();
			EffectLibrary = new EffectLibrary();
			PreconditionLibrary = new PreconditionLibrary();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add a new node schema.
		/// </summary>
		/// <param name="schema"></param>
		public void AddNodeSchema(NodeSchema schema)
		{
			_nodeSchema[schema.nodeType] = schema;
		}

		/// <summary>
		/// Add a new relationship schema.
		/// </summary>
		/// <param name="schema"></param>
		public void AddRelationshipSchema(RelationshipSchema schema)
		{
			_relationshipSchema[(schema.ownerType, schema.targetType)] = schema;
		}

		/// <summary>
		/// Retrieves the social entity with the given ID or creates one
		/// if one is not found.
		/// </summary>
		/// <param name="nodeType"></param>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		public TDRSNode CreateNode(string nodeType, string nodeId)
		{
			if (_nodes.ContainsKey(nodeId)) return _nodes[nodeId];

			if (!_nodeSchema.ContainsKey(nodeType))
			{
				var ex = new KeyNotFoundException($"No schema found for node type: {nodeType}");
				ex.Data["nodeType"] = nodeType;
				throw ex;
			}

			var schema = _nodeSchema[nodeType];
			var node = new TDRSNode(this, nodeType, nodeId);

			foreach (var entry in schema.stats)
			{
				node.Stats.AddStat(entry.statName, new StatSystem.Stat(
					entry.baseValue, entry.minValue, entry.maxValue, entry.isDiscrete
				));
			}

			_nodes[nodeId] = node;

			return node;
		}

		/// <summary>
		/// Creates gets the relationship from the owner to target and creates a
		/// new relationship if one does not exist.
		///
		/// <para>
		/// This adds the necessary stats active social rules when creating new relationships
		/// </para>
		/// </summary>
		/// <param name="ownerId"></param>
		/// <param name="targetId"></param>
		/// <returns></returns>
		public TDRSRelationship CreateRelationship(string ownerId, string targetId)
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

			if (owner.OutgoingRelationships.ContainsKey(target))
			{
				return owner.OutgoingRelationships[target];
			}

			if (!_relationshipSchema.ContainsKey((owner.NodeType, target.NodeType)))
			{
				throw new KeyNotFoundException(
					"Cannot find relationship schema for connecting a "
					+ $"{owner.NodeType} to a {target.NodeType}.");
			}

			var schema = _relationshipSchema[(owner.NodeType, target.NodeType)];

			var relationship = new TDRSRelationship(
				this, $"{ownerId}->{targetId}", owner, target
			);

			owner.OutgoingRelationships[target] = relationship;
			target.IncomingRelationships[owner] = relationship;

			// Configure stats
			foreach (var entry in schema.stats)
			{
				relationship.Stats.AddStat(entry.statName, new StatSystem.Stat(
					entry.baseValue, entry.minValue, entry.maxValue, entry.isDiscrete
				));
			}

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

			return relationship;
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
