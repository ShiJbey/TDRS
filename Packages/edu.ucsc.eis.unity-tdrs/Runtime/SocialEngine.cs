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
		#region Fields

		protected Queue<SocialRelationship> m_relationshipQueue;
		protected Dictionary<string, SocialAgent> m_agents;
		protected Dictionary<(string, string), SocialRelationship> m_relationships;
		[SerializeField]
		protected EffectFactories m_effectFactories;
		[SerializeField]
		protected SocialEventLibrary m_socialEventLibrary;
		[SerializeField]
		protected TraitLibrary m_traitLibrary;

		#endregion

		#region Properties

		public static SocialEngine Instance { get; private set; }
		public TraitLibrary TraitLibrary => m_traitLibrary;
		public EffectFactories EffectFactories => m_effectFactories;
		public List<SocialAgent> Agents => m_agents.Values.ToList();
		public RePraxisDatabase DB { get; protected set; }
		public SocialEventLibrary SocialEventLibrary => m_socialEventLibrary;

		#endregion

		#region Unity Messages

		private void Awake()
		{
			// Ensure there is only one instance of this MonoBehavior active within the scene
			if (Instance != null && Instance != this)
			{
				Debug.LogError(
					"Only on SocialEngine may be active in a scene. Destroying this one."
				);
				Destroy(this);
			}
			else
			{
				Instance = this;
				m_relationshipQueue = new Queue<SocialRelationship>();
				DB = new RePraxisDatabase();
				m_agents = new Dictionary<string, SocialAgent>();
				m_relationships = new Dictionary<(string, string), SocialRelationship>();
			}
		}

		private void Start()
		{
			m_effectFactories.RegisterFactories();
			m_traitLibrary.LoadTraitDefinitions();
			m_socialEventLibrary.LoadEventDefinitions();
		}

		private void Update()
		{
			ProcessRelationshipQueue();
		}

		#endregion

		#region Public Methods

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

			}

			// Apply incoming social rules from the target
			foreach (var rule in target.SocialRules.Rules)
			{

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


		/// <summary>
		/// Dispatch an event throughout the social network and apply effects
		/// </summary>
		/// <param name="socialEvent"></param>
		public void DispatchEvent(string eventName, params string[] agents)
		{
			// Get the event type definition from the library
			var eventType = m_socialEventLibrary.GetEventType($"{eventName}/{agents.Length}");

			// Create the base context for the events
			var ctx = new EffectBindingContext(this, eventType, agents);

			// Iterate through the responses
			foreach (var response in eventType.Responses)
			{
				if (response.Query != null)
				{
					var results = response.Query.Run(ctx.Engine.DB, ctx.Bindings);

					// Skip this response because the query failed
					if (!results.Success) continue;

					// Create a new context for each binding result
					foreach (var bindingSet in results.Bindings)
					{
						var scopedCtx = ctx.WithBindings(bindingSet);

						foreach (var effectEntry in response.Effects)
						{
							// Separate the entry into multiple parts
							List<string> effectParts = effectEntry
								.Split(" ").Select(s => s.Trim()).ToList();

							string effectName = effectParts[0]; // The effect name is the first part
							effectParts.RemoveAt(0); // Remove the name from the front of the list

							// Get the factory
							var effectFactory = m_effectFactories.GetEffectFactory(effectName);

							var effect = effectFactory.CreateInstance(
								scopedCtx, effectParts.ToArray()
							);

							effect.Apply();
						}
					}
				}
				else
				{
					foreach (var effectEntry in response.Effects)
					{
						// Separate the entry into multiple parts
						List<string> effectParts = effectEntry
							.Split(" ").Select(s => s.Trim()).ToList();

						string effectName = effectParts[0]; // The effect name is the first part
						effectParts.RemoveAt(0); // Remove the name from the front of the list

						// Get the factory
						var effectFactory = m_effectFactories.GetEffectFactory(effectName);

						var effect = effectFactory.CreateInstance(ctx, effectParts.ToArray());

						effect.Apply();
					}
				}
			}
		}

		/// <summary>
		/// Advance the simulation by one simulation tick
		/// </summary>
		public void Tick()
		{
			foreach (var agent in m_agents.Values)
			{
				agent.Tick();
			}

			foreach (var relationship in m_relationships.Values)
			{
				relationship.Tick();
			}
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

		#endregion
	}
}
