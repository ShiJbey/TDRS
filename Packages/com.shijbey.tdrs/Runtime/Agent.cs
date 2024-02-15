using System;
using System.Collections.Generic;
using RePraxis;

namespace TDRS
{
	/// <summary>
	/// An entity within a social graph that is connected to other agents via relationships.
	/// </summary>
	public class Agent : IEffectable
	{
		#region Properties

		/// <summary>
		/// Get the unique ID of the agent.
		/// </summary>
		public string UID { get; }

		/// <summary>
		/// The type config name of this agent.
		/// </summary>
		public string AgentType { get; }

		/// <summary>
		/// A reference to the manager that owns this agent.
		/// </summary>
		public SocialEngine Engine { get; }

		/// <summary>
		/// The collection of traits associated with this agent.
		/// </summary>
		public TraitManager Traits { get; }

		/// <summary>
		/// A collection of stats associated with this agent.
		/// </summary>
		public StatManager Stats { get; }

		/// <summary>
		/// All social rules affecting this agent.
		/// </summary>
		public SocialRuleManager SocialRules { get; }

		/// <summary>
		/// Manages all effects applied to this agent.
		/// </summary>
		public EffectManager Effects { get; }

		/// <summary>
		/// Relationships directed toward this agent.
		/// </summary>
		public Dictionary<Agent, Relationship> IncomingRelationships { get; }

		/// <summary>
		/// Relationships from this agent directed toward other agents.
		/// </summary>
		public Dictionary<Agent, Relationship> OutgoingRelationships { get; }

		#endregion

		#region Events

		/// <summary>
		/// Event invoked when an agent is ticked.
		/// </summary>
		public event EventHandler OnTick;

		#endregion

		#region Constructors

		public Agent(SocialEngine engine, string uid, string agentType)
		{
			UID = uid;
			AgentType = agentType;
			Engine = engine;
			Traits = new TraitManager();
			Stats = new StatManager();
			SocialRules = new SocialRuleManager();
			Effects = new EffectManager();
			OutgoingRelationships = new Dictionary<Agent, Relationship>();
			IncomingRelationships = new Dictionary<Agent, Relationship>();

			Stats.OnValueChanged += HandleStatChanged;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a trait to the agent.
		/// </summary>
		/// <param name="traitID"></param>
		/// <param name="duration"></param>
		/// <returns></returns>
		public bool AddTrait(string traitID)
		{
			Trait trait = Engine.TraitLibrary.Traits[traitID];

			if (Traits.HasTrait(trait)) return false;

			if (Traits.HasConflictingTrait(trait)) return false;

			if (trait.TraitType != TraitType.Agent)
			{
				throw new ArgumentException(
					$"Trait ({traitID}) must be of type 'Agent'."
				);
			}

			EffectContext ctx = new EffectContext(
				Engine,
				trait.Description,
				new Dictionary<string, object>()
				{
					{ "?owner", UID }
				},
				trait
			);

			TraitInstance traitInstance = TraitInstance.CreateInstance(Engine, trait, ctx, this);

			Engine.DB.Insert($"{UID}.traits.{traitID}");

			Traits.AddTrait(traitInstance);

			SocialRules.AddSource(traitInstance);

			traitInstance.Apply();

			ReevaluateRelationships();

			return true;
		}

		/// <summary>
		/// Remove a trait from the agent.
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public bool RemoveTrait(string traitID)
		{
			if (!Traits.HasTrait(traitID)) return false;

			var traitInstance = Traits.GetTrait(traitID);

			Traits.RemoveTrait(traitID);

			Engine.DB.Delete($"{UID}.traits.{traitID}");

			SocialRules.RemoveSource(traitInstance);

			traitInstance.Remove();

			ReevaluateRelationships();

			return true;
		}

		/// <summary>
		/// Advance the simulation by one simulation tick
		/// </summary>
		public void Tick()
		{
			Effects.TickEffects();

			List<TraitInstance> traitInstanceList = new List<TraitInstance>(Traits.Traits);

			foreach (var traitInstance in traitInstanceList)
			{
				if (!Traits.HasTrait(traitInstance.TraitID)) continue;
				traitInstance.TickSocialRuleInstances();
			}

			OnTick?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Check and recalculate social rule effects.
		/// </summary>
		public void ReevaluateRelationships()
		{
			foreach (var socialRuleSource in SocialRules.Sources)
			{
				foreach (var socialRule in socialRuleSource.SocialRules)
				{
					// Try to apply the social rule to existing outgoing relationships
					foreach (var (other, relationship) in OutgoingRelationships)
					{
						if (
							socialRuleSource.HasSocialRuleInstance(
								socialRule,
								UID,
								other.UID
							)
						)
						{
							var instance = socialRuleSource.GetSocialRuleInstance(
								socialRule, UID, other.UID
							);

							var isValid = new DBQuery(socialRule.Preconditions).Run(
								Engine.DB,
								instance.Context.Bindings
							).Success;

							if (!isValid)
							{
								instance.Remove();
								socialRuleSource.RemoveSocialRuleInstance(instance);
							}

							continue;
						}

						var results = new DBQuery(socialRule.Preconditions).Run(
								Engine.DB,
								new Dictionary<string, object>()
								{
									{"?owner", UID},
									{"?other", other.UID}
								}
							);

						if (!results.Success) continue;

						EffectContext ctx = new EffectContext(
							Engine,
							socialRule.DescriptionTemplate,
							new Dictionary<string, object>(){
								{"?owner", UID},
								{"?other", other.UID}
							},
							socialRule.Source
						);

						var ruleInstance = SocialRuleInstance.Instantiate(socialRule, ctx);

						socialRuleSource.AddSocialRuleInstance(ruleInstance);

						ruleInstance.Apply();
					}

					foreach (var (other, relationship) in IncomingRelationships)
					{
						if (
							socialRuleSource.HasSocialRuleInstance(
								socialRule,
								UID,
								other.UID
							)
						)
						{
							var instance = socialRuleSource.GetSocialRuleInstance(
								socialRule, UID, other.UID
							);

							var isValid = new DBQuery(socialRule.Preconditions).Run(
								Engine.DB,
								instance.Context.Bindings
							).Success;

							if (!isValid)
							{
								instance.Remove();
								socialRuleSource.RemoveSocialRuleInstance(instance);
							}

							continue;
						}

						var results = new DBQuery(socialRule.Preconditions).Run(
							Engine.DB,
							new Dictionary<string, object>()
							{
								{"?owner", UID},
								{"?other", other.UID}
							}
						);

						if (!results.Success) continue;

						EffectContext ctx = new EffectContext(
							Engine,
							socialRule.DescriptionTemplate,
							new Dictionary<string, object>(){
								{"?owner", UID},
								{"?other", other.UID}
							},
							socialRule.Source
						);

						var ruleInstance = SocialRuleInstance.Instantiate(socialRule, ctx);

						socialRuleSource.AddSocialRuleInstance(ruleInstance);

						ruleInstance.Apply();
					}
				}
			}
		}

		public override string ToString()
		{
			return $"Agent({UID})";
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Callback executed when an agent's stat changes its value.
		/// </summary>
		/// <param name="stats"></param>
		/// <param name="nameAndValue"></param>
		private void HandleStatChanged(object stats, StatManager.OnValueChangedArgs args)
		{
			Engine.DB.Insert($"{UID}.stats.{args.StatName}!{args.Value}");
		}

		#endregion
	}
}
