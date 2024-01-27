using System;
using System.Collections.Generic;
using TDRS.StatSystem;

namespace TDRS
{
	/// <summary>
	/// An entity within a social graph that is connected to other nodes via
	/// <c>RelationshipEdge</c> instances.
	/// </summary>
	public class AgentNode
	{
		#region Properties

		/// <summary>
		/// Get the unique ID of the entity
		/// </summary>
		public string UID { get; }

		/// <summary>
		/// The type config name of this agent node
		/// </summary>
		public string NodeType { get; }

		/// <summary>
		/// A reference to the manager that owns this entity
		/// </summary>
		public SocialEngine Engine { get; }

		/// <summary>
		/// The collection of traits associated with this entity
		/// </summary>
		public TraitManager Traits { get; }

		/// <summary>
		/// A collection of stats associated with this entity
		/// </summary>
		public StatManager Stats { get; }

		/// <summary>
		/// All social rules affecting this entity
		/// </summary>
		public SocialRuleManager SocialRules { get; }

		/// <summary>
		/// Relationships directed toward this entity
		/// </summary>
		public Dictionary<AgentNode, RelationshipEdge> IncomingRelationships { get; }

		/// <summary>
		/// Relationships from this entity directed toward other entities
		/// </summary>
		public Dictionary<AgentNode, RelationshipEdge> OutgoingRelationships { get; }

		#endregion

		#region Events

		/// <summary>
		/// Event invoked when an entity is ticked;
		/// </summary>
		public event EventHandler OnTick;

		#endregion

		#region Constructors

		public AgentNode(SocialEngine engine, string uid, string nodeType)
		{
			UID = uid;
			NodeType = nodeType;
			Engine = engine;
			Traits = new TraitManager();
			Stats = new StatManager();
			SocialRules = new SocialRuleManager();
			OutgoingRelationships = new Dictionary<AgentNode, RelationshipEdge>();
			IncomingRelationships = new Dictionary<AgentNode, RelationshipEdge>();

			Stats.OnValueChanged += HandleStatChanged;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a trait to an entity.
		/// </summary>
		/// <param name="traitID"></param>
		/// <param name="duration"></param>
		public void AddTrait(string traitID, int duration = -1)
		{
			if (Traits.HasTrait(traitID)) return;

			Trait trait = Engine.TraitLibrary.CreateInstance(traitID, this);
			Traits.AddTrait(trait, duration);
			Engine.DB.Insert($"{UID}.traits.{traitID}");

			// Apply the trait's effects on the owner
			foreach (var effect in trait.Effects)
			{
				effect.Apply();
			}

			// Add the social rules for this trait
			foreach (var socialRule in trait.SocialRuleDefinitions)
			{
				SocialRules.AddSocialRuleDefinition(socialRule);

				// Try to apply the social rule to existing outgoing relationships
				foreach (var (other, relationship) in OutgoingRelationships)
				{
					if (SocialRules.HasSocialRuleInstance(socialRule, UID, other.UID))
					{
						continue;
					}

					if (socialRule.Query != null)
					{
						var results = socialRule.Query.Run(
							Engine.DB,
							new Dictionary<string, string>()
							{
								{"?owner", UID},
								{"?other", other.UID}
							}
						);

						if (!results.Success) continue;

						foreach (var result in results.Bindings)
						{
							var ctx = new EffectBindingContext(
								Engine,
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
								SocialRules.AddSocialRuleInstance(ruleInstance);
							}
						}
					}
					else
					{
						var ctx = new EffectBindingContext(
							Engine,
							socialRule.DescriptionTemplate,
							new Dictionary<string, string>()
							{
								{"?owner", UID},
								{"?other", other.UID}
							}
						);

						var ruleInstance = SocialRuleInstance.TryInstantiateRule(socialRule, ctx);

						if (ruleInstance != null)
						{
							SocialRules.AddSocialRuleInstance(ruleInstance);
						}
					}
				}

				// Try to apply the social rule to existing incoming relationships
				foreach (var (other, relationship) in IncomingRelationships)
				{
					if (SocialRules.HasSocialRuleInstance(socialRule, other.UID, UID))
					{
						continue;
					}

					if (socialRule.Query != null)
					{
						var results = socialRule.Query.Run(
							Engine.DB,
							new Dictionary<string, string>()
							{
								{"?owner", UID},
								{"?other", other.UID}
							}
						);

						if (!results.Success) continue;

						foreach (var result in results.Bindings)
						{
							var ctx = new EffectBindingContext(
								Engine,
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
								SocialRules.AddSocialRuleInstance(ruleInstance);
							}
						}
					}
					else
					{
						var ctx = new EffectBindingContext(
							Engine,
							socialRule.DescriptionTemplate,
							new Dictionary<string, string>()
							{
								{"?owner", UID},
								{"?other", other.UID}
							}
						);

						var ruleInstance = SocialRuleInstance.TryInstantiateRule(socialRule, ctx);

						if (ruleInstance != null)
						{
							SocialRules.AddSocialRuleInstance(ruleInstance);
						}
					}
				}
			}
		}

		/// <summary>
		/// Remove a trait from the entity.
		/// </summary>
		/// <param name="traitID"></param>
		public void RemoveTrait(string traitID)
		{
			var trait = Traits.GetTrait(traitID);
			Traits.RemoveTrait(trait);
			Engine.DB.Delete($"{UID}.traits.{traitID}");

			// Undo the effects of the trait on the owner
			foreach (var effect in trait.Effects)
			{
				effect.Remove();
			}

			// Remove all the social rules
			foreach (var socialRule in trait.SocialRuleDefinitions)
			{
				SocialRules.RemoveSocialRuleDefinition(socialRule);
			}
		}

		/// <summary>
		/// Advance the simulation by one simulation tick
		/// </summary>
		public void Tick()
		{
			TickTraits();
			TickStats();

			OnTick?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Update the stats and modifiers by one simulation tick
		/// </summary>
		public void TickStats()
		{
			List<StatModifier> modifiers = new List<StatModifier>(Stats.Modifiers);

			// Loop backward since we may remove items from the list
			foreach (var modifier in modifiers)
			{
				if (modifier.Duration > 0)
				{
					modifier.DecrementDuration();
				}

				if (modifier.Duration == 0)
				{
					Stats.RemoveModifier(modifier);
				}
			}
		}

		/// <summary>
		/// Tick update the traits of characters
		/// </summary>
		public void TickTraits()
		{
			List<Trait> traits = Traits.Traits;

			// Loop backward since we may remove items from the list
			for (int i = traits.Count - 1; i >= 0; i--)
			{
				var trait = traits[i];

				if (trait.Duration > 0)
				{
					trait.DecrementDuration();
				}

				if (trait.Duration == 0)
				{
					RemoveTrait(trait.TraitID);
				}
			}
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Callback executed when an agent's stat changes its value.
		/// </summary>
		/// <param name="stats"></param>
		/// <param name="nameAndValue"></param>
		private void HandleStatChanged(object stats, (string, float) nameAndValue)
		{
			string statName = nameAndValue.Item1;
			float value = nameAndValue.Item2;
			Engine.DB.Insert($"{UID}.stats.{statName}!{value}");
		}

		#endregion
	}
}
