using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RePraxis;


namespace TDRS
{
	/// <summary>
	/// This is the main entry point and overall manager for all information related
	/// to the Trait-Driven Relationship System.
	///
	/// <para>
	/// This MonoBehaviour is responsible for managing information about all the
	/// traits, preconditions, and effects.
	/// </para>
	///
	/// <para>
	/// This is a singleton class. Only one TDRSManager should be present in a scene.
	/// </para>
	/// </summary>
	[DefaultExecutionOrder(-5)]
	public class SocialEngine : MonoBehaviour
	{
		#region Attributes

		/// <summary>
		/// A list of TextAssets assigned within the Unity inspector
		/// </summary>
		[SerializeField]
		protected List<TextAsset> _traitDefinitions = new List<TextAsset>();
		protected Queue<SocialRelationship> relationshipQueue;
		protected Dictionary<string, SocialAgent> _nodes;
		protected Dictionary<(string, string), SocialRelationship> _relationships;

		#endregion

		#region Properties

		public static SocialEngine Instance { get; private set; }
		public TraitLibrary TraitLibrary { get; protected set; }
		public EffectLibrary EffectLibrary { get; protected set; }
		public PreconditionLibrary PreconditionLibrary { get; protected set; }
		public IEnumerable<SocialAgent> Nodes => _nodes.Values;
		public RePraxisDatabase DB { get; protected set; }

		#endregion

		#region Events

		public LoadFactoriesEvent OnLoadFactories;

		#endregion

		#region Unity Methods
		private void Awake()
		{
			// Ensure there is only one instance of this MonoBehavior active within the scene
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
				relationshipQueue = new Queue<SocialRelationship>();
				TraitLibrary = new TraitLibrary();
				EffectLibrary = new EffectLibrary();
				PreconditionLibrary = new PreconditionLibrary();
				DB = new RePraxisDatabase();
				_nodes = new Dictionary<string, SocialAgent>();
				_relationships = new Dictionary<(string, string), SocialRelationship>();
			}
		}

		private void Start()
		{
			LoadFactories();
			LoadTraits();
		}

		private void Update()
		{
			ProcessRelationshipQueue();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Register a new entity with the manager.
		/// </summary>
		/// <param name="agent"></param>
		public bool RegisterAgent(SocialAgent agent)
		{
			if (_nodes.ContainsKey(agent.UID))
			{
				throw new Exception($"Agent already exists with ID: {agent.UID}");
			}

			_nodes[agent.UID] = agent;

			DB.Insert($"{agent.UID}");

			foreach (var entry in agent.StatSchema.stats)
			{
				agent.Stats.AddStat(
					entry.statName,
					new StatSystem.Stat(
						entry.baseValue, entry.minValue, entry.maxValue, entry.isDiscrete
					)
				);
			}

			// Configure initial traits
			foreach (var traitID in agent.traitsAtStart)
			{
				agent.AddTrait(traitID);
			}

			// Configure initial stats
			foreach (var entry in agent.BaseStats)
			{
				agent.Stats.GetStat(entry.name).BaseValue = entry.baseValue;
			}

			return true;
		}

		/// <summary>
		/// Register a new relationship with the manager.
		/// </summary>
		/// <param name="relationship"></param>
		/// <returns></returns>
		public bool RegisterRelationship(SocialRelationship relationship)
		{
			if (!HasAgent(relationship.Owner.UID))
			{
				relationshipQueue.Enqueue(relationship);
				return false;
			}

			if (!HasAgent(relationship.Target.UID))
			{
				relationshipQueue.Enqueue(relationship);
				return false;
			}

			if (_relationships.ContainsKey((relationship.Owner.UID, relationship.Target.UID)))
			{
				throw new Exception(
					"A relationship already exists between "
					+ $"{relationship.Owner} and {relationship.Target}.");
			}

			_relationships[(relationship.Owner.UID, relationship.Target.UID)] = relationship;

			var owner = relationship.Owner;
			var target = relationship.Target;

			owner.OutgoingRelationships[target] = relationship;
			target.IncomingRelationships[owner] = relationship;

			foreach (var entry in relationship.StatSchema.stats)
			{
				relationship.Stats.AddStat(
					entry.statName,
					new StatSystem.Stat(
						entry.baseValue, entry.minValue, entry.maxValue, entry.isDiscrete
					)
				);
			}

			// Configure initial stats
			foreach (var entry in relationship.BaseStats)
			{
				relationship.Stats.GetStat(entry.name).BaseValue = entry.baseValue;
			}

			// Configure initial traits
			foreach (var traitID in relationship.traitsAtStart)
			{
				relationship.AddTrait(traitID);
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

			DB.Insert($"{owner.UID}.relationship.{target.UID}");

			return true;
		}

		/// <summary>
		/// Get a reference to a node.
		/// </summary>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException">If no node found with given ID.</exception>
		public SocialAgent GetAgent(string nodeId)
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
		public SocialRelationship GetRelationship(string ownerId, string targetId)
		{
			if (!_nodes.ContainsKey(ownerId))
			{
				throw new KeyNotFoundException($"Cannot find node with ID: {ownerId}.");
			}

			if (!_nodes.ContainsKey(targetId))
			{
				throw new KeyNotFoundException($"Cannot find node with ID: {targetId}.");
			}

			var owner = GetAgent(ownerId);
			var target = GetAgent(targetId);

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
		public bool HasAgent(string nodeId)
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
		public bool TryGetAgent(string nodeId, out SocialAgent node)
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
			out SocialRelationship relationship)
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

		#region Private Methods

		private void ProcessRelationshipQueue()
		{
			List<SocialRelationship> relationships = new List<SocialRelationship>(relationshipQueue);
			relationshipQueue.Clear();

			foreach (var relationship in relationships)
			{
				RegisterRelationship(relationship);
			}
		}

		private void LoadFactories()
		{
			OnLoadFactories.Invoke(this);
		}

		private void LoadTraits()
		{
			foreach (var textAsset in _traitDefinitions)
			{
				TraitLibrary.LoadTraits(textAsset.text);
			}

			TraitLibrary.InstantiateTraits(this);
		}

		#endregion
	}


	#region Custom Event Classes

	[Serializable]
	/// <summary>
	/// Event dispatched when the TDRSmanager is loading factory instances during start.
	/// </summary>
	public class LoadFactoriesEvent : UnityEvent<SocialEngine> { }

	#endregion
}
