using System;
using System.Collections.Generic;
using UnityEngine;
using RePraxis;
using System.Linq;


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
		protected List<TextAsset> m_traitDefinitions;
		protected Queue<SocialRelationship> m_relationshipQueue;
		protected Dictionary<string, SocialAgent> m_agents;
		protected Dictionary<(string, string), SocialRelationship> m_relationships;

		[SerializeField]
		private List<EffectFactoryEntry> m_effectFactories;

		[SerializeField]
		private List<PreconditionFactoryEntry> m_preconditionFactories;

		#endregion

		#region Properties

		public static SocialEngine Instance { get; private set; }
		public TraitLibrary TraitLibrary { get; protected set; }
		public EffectLibrary EffectLibrary { get; protected set; }
		public PreconditionLibrary PreconditionLibrary { get; protected set; }
		public List<SocialAgent> Agents => m_agents.Values.ToList();
		public RePraxisDatabase DB { get; protected set; }

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
				m_relationshipQueue = new Queue<SocialRelationship>();
				TraitLibrary = new TraitLibrary();
				EffectLibrary = new EffectLibrary();
				PreconditionLibrary = new PreconditionLibrary();
				DB = new RePraxisDatabase();
				m_agents = new Dictionary<string, SocialAgent>();
				m_relationships = new Dictionary<(string, string), SocialRelationship>();
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
			if (m_agents.ContainsKey(agent.UID))
			{
				throw new Exception($"Agent already exists with ID: {agent.UID}");
			}

			m_agents[agent.UID] = agent;

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
			foreach (var traitID in agent.BaseTraits)
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
				m_relationshipQueue.Enqueue(relationship);
				return false;
			}

			if (!HasAgent(relationship.Target.UID))
			{
				m_relationshipQueue.Enqueue(relationship);
				return false;
			}

			if (m_relationships.ContainsKey((relationship.Owner.UID, relationship.Target.UID)))
			{
				throw new Exception(
					"A relationship already exists between "
					+ $"{relationship.Owner} and {relationship.Target}.");
			}

			m_relationships[(relationship.Owner.UID, relationship.Target.UID)] = relationship;

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
			foreach (var traitID in relationship.BaseTraits)
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
		/// <param name="agentID"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException">If no node found with given ID.</exception>
		public SocialAgent GetAgent(string agentID)
		{
			if (!m_agents.ContainsKey(agentID))
			{
				throw new KeyNotFoundException($"Cannot find node with ID: {agentID}.");
			}

			return m_agents[agentID];
		}

		/// <summary>
		/// Get a reference to a relationship.
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="targetID"></param>
		/// <returns></returns>
		public SocialRelationship GetRelationship(string ownerID, string targetID)
		{
			if (!m_agents.ContainsKey(ownerID))
			{
				throw new KeyNotFoundException($"Cannot find node with ID: {ownerID}.");
			}

			if (!m_agents.ContainsKey(targetID))
			{
				throw new KeyNotFoundException($"Cannot find node with ID: {targetID}.");
			}

			var owner = GetAgent(ownerID);
			var target = GetAgent(targetID);

			if (!owner.OutgoingRelationships.ContainsKey(target))
			{
				throw new KeyNotFoundException(
					$"Cannot find relationship from {ownerID} to {targetID}.");
			}

			return owner.OutgoingRelationships[target];
		}

		/// <summary>
		/// Check if a node exists
		/// </summary>
		/// <param name="agentID"></param>
		/// <returns></returns>
		public bool HasAgent(string agentID)
		{
			return m_agents.ContainsKey(agentID);
		}

		/// <summary>
		/// Check if a relationship exists
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="targetID"></param>
		/// <returns></returns>
		public bool HasRelationship(string ownerID, string targetID)
		{
			if (!m_agents.ContainsKey(ownerID)) return false;
			if (!m_agents.ContainsKey(targetID)) return false;

			var targetNode = m_agents[targetID];

			return m_agents[ownerID].OutgoingRelationships.ContainsKey(targetNode);
		}

		/// <summary>
		/// Try to get a reference to a node
		/// </summary>
		/// <param name="agentID"></param>
		/// <param name="agent"></param>
		/// <returns></returns>
		public bool TryGetAgent(string agentID, out SocialAgent agent)
		{
			agent = null;

			if (!m_agents.ContainsKey(agentID)) return false;

			agent = m_agents[agentID];
			return true;
		}

		/// <summary>
		/// Try to get a reference to a relationship
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="targetID"></param>
		/// <param name="relationship"></param>
		/// <returns></returns>
		public bool TryGetRelationship(
			string ownerID,
			string targetID,
			out SocialRelationship relationship)
		{
			relationship = null;

			if (!m_agents.ContainsKey(ownerID)) return false;
			if (!m_agents.ContainsKey(targetID)) return false;

			var ownerNode = m_agents[ownerID];
			var targetNode = m_agents[targetID];

			if (!ownerNode.OutgoingRelationships.ContainsKey(targetNode)) return false;

			relationship = m_agents[ownerID].OutgoingRelationships[targetNode];
			return true;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Iterate over the relationship queue and try to register pending relationships.
		/// </summary>
		private void ProcessRelationshipQueue()
		{
			List<SocialRelationship> relationships =
				new List<SocialRelationship>(m_relationshipQueue);

			m_relationshipQueue.Clear();

			foreach (var relationship in relationships)
			{
				RegisterRelationship(relationship);
			}
		}

		/// <summary>
		/// Load the various factory instances into their respective libraries.
		/// </summary>
		private void LoadFactories()
		{
			foreach (var entry in m_preconditionFactories)
			{
				PreconditionLibrary.AddFactory(entry.m_preconditionType, entry.m_factory);
			}

			foreach (var entry in m_effectFactories)
			{
				EffectLibrary.AddFactory(entry.m_effectType, entry.m_factory);
			}
		}

		/// <summary>
		/// Load traits from the text assets provided in the inspector.
		/// </summary>
		private void LoadTraits()
		{
			foreach (var textAsset in m_traitDefinitions)
			{
				TraitLibrary.LoadTraits(textAsset.text);
			}

			TraitLibrary.InstantiateTraits(this);
		}

		#endregion

		#region Helper Classes

		/// <summary>
		/// Helper class for organizing effect factories in the inspector
		/// </summary>
		[Serializable]
		public class EffectFactoryEntry
		{
			public string m_effectType;
			public EffectFactory m_factory;
		}

		/// <summary>
		/// Helper class for organizing precondition factories in the inspector.
		/// </summary>
		[Serializable]
		public class PreconditionFactoryEntry
		{
			public string m_preconditionType;
			public PreconditionFactory m_factory;
		}

		#endregion
	}
}
