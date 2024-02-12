using System.Linq;
using System.Collections.Generic;
using RePraxis;
using System;

namespace TDRS
{
	public class SocialEngine
	{
		#region Fields

		/// <summary>
		/// A lookup table of UIDs mapped to their agent instances.
		/// </summary>
		private Dictionary<string, Agent> m_agents;

		/// <summary>
		/// A lookup table of owner/target UID tuples mapped to relationship instances.
		/// </summary>
		private Dictionary<(string, string), Relationship> m_relationships;

		#endregion

		#region Properties

		/// <summary>
		/// All agents.
		/// </summary>
		public IEnumerable<Agent> Agents => m_agents.Values;

		/// <summary>
		/// All relationships between agents.
		/// </summary>
		public IEnumerable<Relationship> Relationships => m_relationships.Values;

		/// <summary>
		/// A library of social event definitions.
		/// </summary>
		public SocialEventLibrary SocialEventLibrary { get; private set; }

		/// <summary>
		/// Manages all definition information for available traits.
		/// </summary>
		public TraitLibrary TraitLibrary { get; private set; }

		/// <summary>
		/// Manages factory instances used to instantiate effects for traits, social rules, and
		/// social events.
		/// </summary>
		public EffectLibrary EffectLibrary { get; private set; }

		/// <summary>
		/// The database where queryable relationship information is stored.
		/// </summary>
		public RePraxisDatabase DB { get; private set; }

		/// <summary>
		/// A lookup table of agent type names mapped to configuration settings.
		/// </summary>
		public Dictionary<string, AgentConfig> AgentConfigs { get; private set; }

		/// <summary>
		/// A lookup table of relationship owner/target type name tuples mapped to
		/// relationship configuration settings.
		/// </summary>
		public Dictionary<(string, string), RelationshipConfig> RelationshipConfigs { get; private set; }

		#endregion

		#region Actions and Events

		public event EventHandler<OnNewAgentArgs> OnNewAgent;
		public class OnNewAgentArgs : EventArgs
		{
			public Agent agent;
		}

		public event EventHandler<OnNewRelationshipArgs> OnNewRelationship;
		public class OnNewRelationshipArgs : EventArgs
		{
			public Relationship relationship;
		}

		#endregion

		#region Constructors

		private SocialEngine()
		{
			m_agents = new Dictionary<string, Agent>();
			m_relationships = new Dictionary<(string, string), Relationship>();
			TraitLibrary = new TraitLibrary();
			SocialEventLibrary = new SocialEventLibrary();
			EffectLibrary = new EffectLibrary();
			DB = new RePraxisDatabase();
			AgentConfigs = new Dictionary<string, AgentConfig>();
			RelationshipConfigs = new Dictionary<(string, string), RelationshipConfig>();
		}

		#endregion

		#region Public Methods

		public Agent AddAgent(string agentType, string uid)
		{
			if (!AgentConfigs.ContainsKey(agentType))
			{
				throw new KeyNotFoundException($"No config found for agent type: {agentType}");
			}

			AgentConfig config = AgentConfigs[agentType];

			Agent agent = new Agent(this, uid, agentType);
			m_agents[uid] = agent;
			DB.Insert($"{uid}");

			// Configure stats
			foreach (StatSchema entry in config.stats)
			{
				agent.Stats.AddStat(
					entry.statName,
					new Stat(
						entry.baseValue, entry.minValue, entry.maxValue, entry.isDiscrete
					)
				);
			}

			// Configure initial traits
			foreach (string traitID in config.traits)
			{
				agent.AddTrait(traitID);
			}

			OnNewAgent?.Invoke(this, new OnNewAgentArgs() { agent = agent });

			return agent;
		}

		public Relationship AddRelationship(string ownerID, string targetID)
		{
			Agent owner = GetAgent(ownerID);
			Agent target = GetAgent(targetID);

			return AddRelationship(owner, target);
		}

		public Relationship AddRelationship(Agent owner, Agent target)
		{
			RelationshipConfig config = RelationshipConfigs[(owner.AgentType, target.AgentType)];

			Relationship relationship = new Relationship(this, owner, target);

			m_relationships[(owner.UID, target.UID)] = relationship;

			owner.OutgoingRelationships[target] = relationship;
			target.IncomingRelationships[owner] = relationship;

			DB.Insert($"{owner.UID}.relationships.{target.UID}");

			// Set stats
			foreach (var entry in config.stats)
			{
				relationship.Stats.AddStat(
					entry.statName,
					new Stat(
						entry.baseValue, entry.minValue, entry.maxValue, entry.isDiscrete
					)
				);
			}

			// Configure initial traits
			foreach (string traitID in config.traits)
			{
				relationship.AddTrait(traitID);
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

			OnNewRelationship?.Invoke(
				this, new OnNewRelationshipArgs() { relationship = relationship });

			return relationship;
		}

		/// <summary>
		/// Get a reference to an agent.
		/// </summary>
		/// <param name="agentID"></param>
		/// <returns></returns>
		public Agent GetAgent(string agentID)
		{
			return m_agents[agentID];
		}

		/// <summary>
		/// Get a reference to a relationship.
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="targetID"></param>
		/// <returns></returns>
		public Relationship GetRelationship(string ownerID, string targetID)
		{
			return m_relationships[(ownerID, targetID)];
		}

		/// <summary>
		/// Check if an agent exists
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
			return m_relationships.ContainsKey((ownerID, targetID));
		}

		/// <summary>
		/// Try to get a reference to an agent.
		/// </summary>
		/// <param name="agentID"></param>
		/// <param name="agent"></param>
		/// <returns></returns>
		public bool TryGetAgent(string agentID, out Agent agent)
		{
			agent = null;

			if (!HasAgent(agentID)) return false;

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
			out Relationship relationship)
		{

			if (!HasRelationship(ownerID, targetID))
			{
				relationship = null;
				return false;
			}

			relationship = m_relationships[(ownerID, targetID)];
			return true;
		}

		/// <summary>
		/// Remove an agent from the social engine.
		/// </summary>
		/// <param name="agentID"></param>
		/// <returns></returns>
		public bool RemoveAgent(string agentID)
		{
			if (!HasAgent(agentID)) return false;

			Agent agent = GetAgent(agentID);

			m_agents.Remove(agentID);



			var outgoingRelationships = agent.OutgoingRelationships.Values.ToList();
			foreach (var relationship in outgoingRelationships)
			{
				RemoveRelationship(relationship.Owner.UID, relationship.Target.UID);
			}

			var incomingRelationships = agent.IncomingRelationships.Values.ToList();
			foreach (var relationship in incomingRelationships)
			{
				RemoveRelationship(relationship.Owner.UID, relationship.Target.UID);
			}

			DB.Delete($"{agent.UID}");

			// agent.Destroy();

			return true;
		}

		/// <summary>
		/// Remove a relationship from the social engine.
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="targetID"></param>
		/// <returns></returns>
		public bool RemoveRelationship(string ownerID, string targetID)
		{
			if (!HasRelationship(ownerID, targetID))
			{
				return false;
			}

			Relationship relationship = GetRelationship(ownerID, targetID);

			// Remove all the traits from the relationship
			List<Trait> traitsList = relationship.Traits.Traits.ToList();
			foreach (var trait in traitsList)
			{
				relationship.RemoveTrait(trait.TraitID);
			}

			//
			relationship.Owner.OutgoingRelationships.Remove(relationship.Target);
			relationship.Target.IncomingRelationships.Remove(relationship.Owner);

			m_relationships.Remove((relationship.Owner.UID, relationship.Target.UID));

			DB.Delete($"{relationship.Owner.UID}.relationships.{relationship.Owner.UID}");

			// relationship.Destroy();

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

		/// <summary>
		/// Removes all agents and relationships from the state.
		/// </summary>
		public void Reset()
		{
			m_agents.Clear();
			m_relationships.Clear();
		}

		#endregion

		#region Static Methods

		public static SocialEngine CreateState()
		{
			return CreateState(null);
		}

		public static SocialEngine CreateState(RePraxisDatabase db)
		{
			var state = new SocialEngine();

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
