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
	/// This is a singleton class. Only one SocialEngine should be present in a scene.
	/// </para>
	/// </summary>
	[DefaultExecutionOrder(-5)]
	public class SocialEngine : MonoBehaviour
	{
		#region Fields

		private Queue<SocialRelationship> m_relationshipQueue;
		private Dictionary<string, AgentNode> m_agents;

		private Dictionary<(string, string), RelationshipEdge> m_relationships;
		[SerializeField]
		private EffectFactories m_effectFactories;
		[SerializeField]
		private SocialEventLibrary m_socialEventLibrary;
		[SerializeField]
		private TraitLibrary m_traitLibrary;

		/// <summary>
		/// Should the social engine not be destroyed when loading a new scene.
		/// <para>
		/// If you're not doing additive scene loading, then this should probably be set to true.
		/// </para>
		/// </summary>
		[SerializeField]
		private bool m_dontDestroyOnLoad;

		/// <summary>
		/// ScriptableObjects containing settings for constructing new nodes
		/// in the social graph.
		/// </summary>
		[SerializeField]
		private List<AgentConfigSO> m_agentConfigAssets;

		/// <summary>
		/// ScriptableObjects containing settings for constructing new relationships
		/// in the social graph.
		/// </summary>
		[SerializeField]
		private List<RelationshipConfigSO> m_relationshipConfigAssets;

		/// <summary>
		/// When true, bi-directional relationships are created between all nodes
		/// in the social graph.
		/// </summary>
		[SerializeField]
		private bool m_isFullyConnectedGraph;

		/// <summary>
		/// A lookup table of agent types mapped to configuration settings.
		/// </summary>
		private Dictionary<string, AgentConfig> m_agentConfigs;

		/// <summary>
		/// A lookup table of relationship owner/target type tuples mapped to
		/// relationship configuration settings.
		/// </summary>
		private Dictionary<(string, string), RelationshipConfig> m_relationshipConfigs;

		#endregion

		#region Properties

		public static SocialEngine Instance { get; private set; }
		public TraitLibrary TraitLibrary => m_traitLibrary;
		public EffectFactories EffectFactories => m_effectFactories;
		public List<AgentNode> Agents => m_agents.Values.ToList();
		public RePraxisDatabase DB { get; private set; }
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
				m_agents = new Dictionary<string, AgentNode>();
				m_relationships = new Dictionary<(string, string), RelationshipEdge>();
				m_agentConfigs = new Dictionary<string, AgentConfig>();
				m_relationshipConfigs = new Dictionary<(string, string), RelationshipConfig>();

				if (m_dontDestroyOnLoad)
				{
					DontDestroyOnLoad(this);
				}
			}
		}

		private void Start()
		{
			LoadAgentConfigs();
			LoadRelationshipConfigs();
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
		public AgentNode RegisterAgent(SocialAgent agent)
		{
			if (m_agents.ContainsKey(agent.UID))
			{
				return m_agents[agent.UID];
			}

			AgentNode node = AddAgent(agent.Config.agentType, agent.UID);

			// Configure initial traits
			foreach (var traitID in agent.BaseTraits)
			{
				node.AddTrait(traitID);
			}

			// Configure initial stats
			foreach (var entry in agent.BaseStats)
			{
				node.Stats.GetStat(entry.name).BaseValue = entry.baseValue;
			}

			return node;
		}

		/// <summary>
		/// Register a new relationship with the manager.
		/// </summary>
		/// <param name="relationship"></param>
		/// <returns></returns>
		public RelationshipEdge RegisterRelationship(SocialRelationship relationship)
		{
			if (!HasAgent(relationship.Owner.UID))
			{
				RegisterAgent(relationship.Owner);
			}

			if (!HasAgent(relationship.Target.UID))
			{
				RegisterAgent(relationship.Target);
			}



			if (m_relationships.ContainsKey((relationship.Owner.UID, relationship.Target.UID)))
			{
				return m_relationships[(
					relationship.Owner.UID,
					relationship.Target.UID)];
			}

			RelationshipEdge relationshipEdge = AddRelationship(
				relationship.Owner.UID,
				relationship.Target.UID);

			// Configure initial stats
			foreach (var entry in relationship.BaseStats)
			{
				relationshipEdge.Stats.GetStat(entry.name).BaseValue = entry.baseValue;
			}

			// Configure initial traits
			foreach (var traitID in relationship.BaseTraits)
			{
				relationshipEdge.AddTrait(traitID);
			}

			return relationshipEdge;
		}

		/// <summary>
		/// Get a reference to a node.
		/// </summary>
		/// <param name="agentID"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException">If no node found with given ID.</exception>
		public AgentNode GetAgent(string agentID)
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
		public RelationshipEdge GetRelationship(string ownerID, string targetID)
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
		public bool TryGetAgent(string agentID, out AgentNode agent)
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
			out RelationshipEdge relationship)
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

						try
						{
							var effects = response.Effects
							.Select(s => m_effectFactories.CreateInstance(scopedCtx, s));

							foreach (var effect in effects)
							{
								effect.Apply();
							}
						}
						catch (ArgumentException ex)
						{
							throw new ArgumentException(
								$"Error encountered while instantiating effects for '{eventName}' event: "
								+ ex.Message
							);
						}
					}
				}
				else
				{
					try
					{
						var effects = response.Effects
						.Select(s => m_effectFactories.CreateInstance(ctx, s));

						foreach (var effect in effects)
						{
							effect.Apply();
						}
					}
					catch (ArgumentException ex)
					{
						throw new ArgumentException(
							$"Error encountered while instantiating effects for '{eventName}' event: "
							+ ex.Message
						);
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

		public void AddAgentConfig(AgentConfig config)
		{
			m_agentConfigs[config.agentType] = config;
		}

		public void AddRelationshipConfig(RelationshipConfig config)
		{
			m_relationshipConfigs[(config.ownerAgentType, config.targetAgentType)] = config;
		}

		public AgentNode AddAgent(string agentType, string uid)
		{
			if (!m_agentConfigs.ContainsKey(agentType))
			{
				throw new KeyNotFoundException($"No config found for agent type: {agentType}");
			}

			AgentConfig config = m_agentConfigs[agentType];

			AgentNode agentNode = new AgentNode(this, uid, agentType);
			m_agents[uid] = agentNode;
			DB.Insert($"{uid}");

			// Configure stats
			foreach (StatSchema entry in config.stats)
			{
				agentNode.Stats.AddStat(
					entry.statName,
					new StatSystem.Stat(
						entry.baseValue, entry.minValue, entry.maxValue, entry.isDiscrete
					)
				);
			}

			// Configure initial traits
			foreach (string traitID in config.traits)
			{
				agentNode.AddTrait(traitID);
			}

			return agentNode;
		}

		public RelationshipEdge AddRelationship(string ownerID, string targetID)
		{
			AgentNode owner = GetAgent(ownerID);
			AgentNode target = GetAgent(targetID);

			if (!m_relationshipConfigs.ContainsKey((owner.NodeType, target.NodeType)))
			{
				throw new KeyNotFoundException(
					$"No relationship config found for agents of types: {owner.NodeType} "
					+ "and {target.NodeType}"
				);
			}

			RelationshipConfig config = m_relationshipConfigs[(owner.NodeType, target.NodeType)];

			RelationshipEdge relationshipEdge = new RelationshipEdge(this, owner, target);

			m_relationships[(ownerID, targetID)] = relationshipEdge;

			owner.OutgoingRelationships[target] = relationshipEdge;
			target.IncomingRelationships[owner] = relationshipEdge;

			DB.Insert($"{owner.UID}.relationships.{target.UID}");

			// Set stats
			foreach (var entry in config.stats)
			{
				relationshipEdge.Stats.AddStat(
					entry.statName,
					new StatSystem.Stat(
						entry.baseValue, entry.minValue, entry.maxValue, entry.isDiscrete
					)
				);
			}

			// Configure initial traits
			foreach (string traitID in config.traits)
			{
				relationshipEdge.AddTrait(traitID);
			}

			// Apply social rules from the owner
			foreach (var socialRule in owner.SocialRules.Rules)
			{
				if (owner.SocialRules.HasSocialRuleInstance(socialRule, owner.UID, target.UID))
				{
					continue;
				}

				if (socialRule.Query != null)
				{
					var results = socialRule.Query.Run(
						DB,
						new Dictionary<string, string>()
						{
							{"?owner", owner.UID},
							{"?other", target.UID}
						}
					);

					if (!results.Success) continue;

					foreach (var result in results.Bindings)
					{
						var ctx = new EffectBindingContext(
							this,
							socialRule.DescriptionTemplate,
							// Here we limit the scope of available variables to only ?owner and ?other
							new Dictionary<string, string>(){
								{"?owner", result["?owner"]},
								{"?other", result["?other"]}
							}
						);

						var ruleInstance = SocialRuleInstance.TryInstantiateRule(socialRule, ctx);

						if (ruleInstance != null)
						{
							owner.SocialRules.AddSocialRuleInstance(ruleInstance);
						}
					}
				}
				else
				{
					var ctx = new EffectBindingContext(
						this,
						socialRule.DescriptionTemplate,
						new Dictionary<string, string>()
						{
							{"?owner", owner.UID},
							{"?other", target.UID}
						}
					);

					var ruleInstance = SocialRuleInstance.TryInstantiateRule(socialRule, ctx);

					if (ruleInstance != null)
					{
						owner.SocialRules.AddSocialRuleInstance(ruleInstance);
					}
				}
			}

			// Apply social rules from the target
			foreach (var socialRule in target.SocialRules.Rules)
			{
				if (target.SocialRules.HasSocialRuleInstance(socialRule, target.UID, owner.UID))
				{
					continue;
				}

				if (socialRule.Query != null)
				{
					var results = socialRule.Query.Run(
						DB,
						new Dictionary<string, string>()
						{
							{"?owner", target.UID},
							{"?other", owner.UID}
						}
					);

					if (!results.Success) continue;

					foreach (var result in results.Bindings)
					{
						var ctx = new EffectBindingContext(
							this,
							socialRule.DescriptionTemplate,
							// Here we limit the scope of available variables to only ?owner and ?other
							new Dictionary<string, string>(){
								{"?owner", result["?owner"]},
								{"?other", result["?other"]}
							}
						);

						var ruleInstance = SocialRuleInstance.TryInstantiateRule(socialRule, ctx);

						if (ruleInstance != null)
						{
							target.SocialRules.AddSocialRuleInstance(ruleInstance);
						}
					}
				}
				else
				{
					var ctx = new EffectBindingContext(
						this,
						socialRule.DescriptionTemplate,
						new Dictionary<string, string>()
						{
							{"?owner", target.UID},
							{"?other", owner.UID}
						}
					);

					var ruleInstance = SocialRuleInstance.TryInstantiateRule(socialRule, ctx);

					if (ruleInstance != null)
					{
						target.SocialRules.AddSocialRuleInstance(ruleInstance);
					}
				}
			}

			return relationshipEdge;
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

		private void LoadAgentConfigs()
		{
			for (int i = 0; i < m_agentConfigAssets.Count; i++)
			{
				AgentConfig config = m_agentConfigAssets[i].CreateAgentConfig();
				m_agentConfigs[config.agentType] = config;
			}
		}

		private void LoadRelationshipConfigs()
		{
			for (int i = 0; i < m_relationshipConfigAssets.Count; i++)
			{
				RelationshipConfig config = m_relationshipConfigAssets[i].CreateRelationshipConfig();
				m_relationshipConfigs[(config.ownerAgentType, config.targetAgentType)] = config;
			}
		}

		private void ReevaluateSocialRules(RelationshipEdge relationship)
		{

		}

		#endregion
	}
}
