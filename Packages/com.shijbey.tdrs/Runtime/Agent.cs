using System;
using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// An entity within a social graph that is connected to other agents via relationships.
	/// </summary>
	public class Agent
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
		public void AddTrait(string traitID)
		{
			if (Traits.HasTrait(traitID)) return;

			Trait trait = Engine.TraitLibrary.Traits[traitID];

			if (trait.TraitType != TraitType.Agent)
			{
				throw new ArgumentException(
					$"Trait ({traitID}) must be of type 'Agent'."
				);
			}

			Traits.AddTrait(trait);
			Engine.DB.Insert($"{UID}.traits.{traitID}");


			EffectBindingContext ctx = new EffectBindingContext(this, trait.Description);

			// Instantiate and apply trait effects
			foreach (var effectEntry in trait.Effects)
			{
				try
				{
					var effect = Engine.EffectLibrary.CreateInstance(ctx, effectEntry);
					effect.Source = trait;
					Effects.AddEffect(effect);
				}
				catch (ArgumentException ex)
				{
					throw new ArgumentException(
						$"Error encountered while instantiating effects for '{traitID}' trait: "
						+ ex.Message
					);
				}
			}

			// // Add the social rules for this trait
			// foreach (var socialRule in trait.SocialRuleDefinitions)
			// {
			// 	SocialRules.AddSocialRuleDefinition(socialRule);

			// 	// Try to apply the social rule to existing outgoing relationships
			// 	foreach (var (other, relationship) in OutgoingRelationships)
			// 	{
			// 		if (SocialRules.HasSocialRuleInstance(socialRule, UID, other.UID))
			// 		{
			// 			continue;
			// 		}

			// 		if (socialRule.Query != null)
			// 		{
			// 			var results = socialRule.Query.Run(
			// 				Engine.DB,
			// 				new Dictionary<string, string>()
			// 				{
			// 					{"?owner", UID},
			// 					{"?other", other.UID}
			// 				}
			// 			);

			// 			if (!results.Success) continue;

			// 			foreach (var result in results.Bindings)
			// 			{
			// 				var ctx = new EffectBindingContext(
			// 					Engine,
			// 					socialRule.DescriptionTemplate,
			// 					// Here we limit the scope of available variables to only ?owner and ?other
			// 					new Dictionary<string, string>(){
			// 						{"?owner", result["?owner"]},
			// 						{"?other", result["?other"]}
			// 					}
			// 				);

			// 				var ruleInstance = SocialRuleInstance.TryInstantiateRule(socialRule, ctx);

			// 				if (ruleInstance != null)
			// 				{
			// 					SocialRules.AddSocialRuleInstance(ruleInstance);
			// 				}
			// 			}
			// 		}
			// 		else
			// 		{
			// 			var ctx = new EffectBindingContext(
			// 				Engine,
			// 				socialRule.DescriptionTemplate,
			// 				new Dictionary<string, string>()
			// 				{
			// 					{"?owner", UID},
			// 					{"?other", other.UID}
			// 				}
			// 			);

			// 			var ruleInstance = SocialRuleInstance.TryInstantiateRule(socialRule, ctx);

			// 			if (ruleInstance != null)
			// 			{
			// 				SocialRules.AddSocialRuleInstance(ruleInstance);
			// 			}
			// 		}
			// 	}

			// 	// Try to apply the social rule to existing incoming relationships
			// 	foreach (var (other, relationship) in IncomingRelationships)
			// 	{
			// 		if (SocialRules.HasSocialRuleInstance(socialRule, other.UID, UID))
			// 		{
			// 			continue;
			// 		}

			// 		if (socialRule.Query != null)
			// 		{
			// 			var results = socialRule.Query.Run(
			// 				Engine.DB,
			// 				new Dictionary<string, string>()
			// 				{
			// 					{"?owner", UID},
			// 					{"?other", other.UID}
			// 				}
			// 			);

			// 			if (!results.Success) continue;

			// 			foreach (var result in results.Bindings)
			// 			{
			// 				var ctx = new EffectBindingContext(
			// 					Engine,
			// 					socialRule.DescriptionTemplate,
			// 					// Here we limit the scope of available variables to only ?owner and ?other
			// 					new Dictionary<string, string>(){
			// 						{"?owner", result["?owner"]},
			// 						{"?other", result["?other"]}
			// 					}
			// 				);

			// 				var ruleInstance = SocialRuleInstance.TryInstantiateRule(socialRule, ctx);

			// 				if (ruleInstance != null)
			// 				{
			// 					SocialRules.AddSocialRuleInstance(ruleInstance);
			// 				}
			// 			}
			// 		}
			// 		else
			// 		{
			// 			var ctx = new EffectBindingContext(
			// 				Engine,
			// 				socialRule.DescriptionTemplate,
			// 				new Dictionary<string, string>()
			// 				{
			// 					{"?owner", UID},
			// 					{"?other", other.UID}
			// 				}
			// 			);

			// 			var ruleInstance = SocialRuleInstance.TryInstantiateRule(socialRule, ctx);

			// 			if (ruleInstance != null)
			// 			{
			// 				SocialRules.AddSocialRuleInstance(ruleInstance);
			// 			}
			// 		}
			// 	}
			// }
		}

		/// <summary>
		/// Remove a trait from the agent.
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public bool RemoveTrait(string traitID)
		{
			if (!Traits.HasTrait(traitID)) return false;

			var trait = Traits.GetTrait(traitID);
			Traits.RemoveTrait(trait);
			Engine.DB.Delete($"{UID}.traits.{traitID}");

			Effects.RemoveAllFromSource(trait);
			SocialRules.RemoveAllSocialRulesFromSource(trait);

			return true;
		}

		/// <summary>
		/// Advance the simulation by one simulation tick
		/// </summary>
		public void Tick()
		{
			Effects.TickEffects();
			OnTick?.Invoke(this, EventArgs.Empty);
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
