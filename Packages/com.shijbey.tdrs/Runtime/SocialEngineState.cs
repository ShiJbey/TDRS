using System.Linq;
using System.Collections.Generic;
using RePraxis;

namespace TDRS
{
	public class SocialEngineState
	{
		#region Fields

		/// <summary>
		/// A lookup table of UIDs mapped to their agent instances.
		/// </summary>
		private Dictionary<string, AgentNode> m_agents;

		/// <summary>
		/// A lookup table of owner/target UID tuples mapped to relationship instances.
		/// </summary>
		private Dictionary<(string, string), RelationshipEdge> m_relationships;

		/// <summary>
		/// A lookup table of agent node type names mapped to configuration settings.
		/// </summary>
		private Dictionary<string, AgentConfig> m_agentConfigs;

		/// <summary>
		/// A lookup table of relationship owner/target node type name tuples mapped to
		/// relationship configuration settings.
		/// </summary>
		private Dictionary<(string, string), RelationshipConfig> m_relationshipConfigs;

		#endregion

		#region Properties

		/// <summary>
		/// All agents.
		/// </summary>
		public IEnumerable<AgentNode> Agents => m_agents.Values;

		/// <summary>
		/// All relationships between agents.
		/// </summary>
		public IEnumerable<RelationshipEdge> Relationships => m_relationships.Values;

		/// <summary>
		/// A library of social event definitions.
		/// </summary>
		public SocialEventLibrary SocialEventLibrary { get; }

		/// <summary>
		/// Manages all definition information for available traits.
		/// </summary>
		public TraitLibrary TraitLibrary { get; }

		/// <summary>
		/// Manages factory instances used to instantiate effects for traits, social rules, and
		/// social events.
		/// </summary>
		public EffectLibrary EffectLibrary;

		/// <summary>
		/// The database where queryable relationship information is stored.
		/// </summary>
		public RePraxisDatabase DB { get; private set; }

		#endregion

		#region Constructors

		private SocialEngineState()
		{
			m_agents = new Dictionary<string, AgentNode>();
			m_relationships = new Dictionary<(string, string), RelationshipEdge>();
			TraitLibrary = new TraitLibrary();
			SocialEventLibrary = new SocialEventLibrary();
			EffectLibrary = new EffectLibrary();
			DB = new RePraxisDatabase();
			m_agentConfigs = new Dictionary<string, AgentConfig>();
			m_relationshipConfigs = new Dictionary<(string, string), RelationshipConfig>();
		}

		#endregion

		#region Public Methods

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

		/// <summary>
		/// Dispatch an event throughout the social network and apply effects
		/// </summary>
		/// <param name="socialEvent"></param>
		public void DispatchEvent(string eventName, params string[] agents)
		{
			// Get the event type definition from the library
			var eventType = SocialEventLibrary.GetSocialEvent($"{eventName}/{agents.Length}");

			// Create the base context for the events
			var ctx = new EffectBindingContext(this, eventType, agents);

			// Iterate through the responses
			foreach (var response in eventType.Responses)
			{
				if (response.Query != null)
				{
					var results = response.Query.Run(ctx.State.DB, ctx.Bindings);

					// Skip this response because the query failed
					if (!results.Success) continue;

					// Create a new context for each binding result
					foreach (var bindingSet in results.Bindings)
					{
						var scopedCtx = ctx.WithBindings(bindingSet);

						try
						{
							var effects = response.Effects
							.Select(s => EffectLibrary.CreateInstance(scopedCtx, s));

							foreach (var effect in effects)
							{
								effect.Apply();
							}
						}
						catch (System.ArgumentException ex)
						{
							throw new System.ArgumentException(
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
						.Select(s => EffectLibrary.CreateInstance(ctx, s));

						foreach (var effect in effects)
						{
							effect.Apply();
						}
					}
					catch (System.ArgumentException ex)
					{
						throw new System.ArgumentException(
							$"Error encountered while instantiating effects for '{eventName}' event: "
							+ ex.Message
						);
					}
				}
			}
		}


		#endregion

		#region Static Methods

		public static SocialEngineState CreateState()
		{
			return CreateState(null);
		}

		public static SocialEngineState CreateState(RePraxisDatabase db)
		{
			var state = new SocialEngineState();

			if (db != null)
			{
				state.DB = db;
			}

			// Add all built-in effect factories
			state.EffectLibrary.AddEffectFactory(new AddAgentTraitFactory());
			state.EffectLibrary.AddEffectFactory(new AddRelationshipTraitFactory());
			state.EffectLibrary.AddEffectFactory(new DecreaseAgentStatFactory());
			state.EffectLibrary.AddEffectFactory(new DecreaseRelationshipStatFactory());
			state.EffectLibrary.AddEffectFactory(new IncreaseAgentStatFactory());
			state.EffectLibrary.AddEffectFactory(new IncreaseRelationshipStatFactory());
			state.EffectLibrary.AddEffectFactory(new RemoveAgentTraitFactory());
			state.EffectLibrary.AddEffectFactory(new RemoveRelationshipTraitFactory());

			return state;
		}

		#endregion
	}
}
