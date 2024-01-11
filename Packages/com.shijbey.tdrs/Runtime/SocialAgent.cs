using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// A user-facing Unity component for associating a GameObject with as node within
	/// the TDRS Manager's social graph.
	/// </summary>
	public class SocialAgent : SocialEntity
	{
		#region Fields

		[SerializeField]
		protected string m_UID;

		#endregion

		#region Properties

		/// <summary>
		/// Get the unique ID of the entity
		/// </summary>
		public string UID => m_UID;

		/// <summary>
		/// All social rules affecting this entity
		/// </summary>
		public SocialRuleManager SocialRules { get; protected set; }

		/// <summary>
		/// Relationships directed toward this entity
		/// </summary>
		public Dictionary<SocialAgent, SocialRelationship> IncomingRelationships
		{
			get; protected set;
		}

		/// <summary>
		/// Relationships from this entity directed toward other entities
		/// </summary>
		public Dictionary<SocialAgent, SocialRelationship> OutgoingRelationships
		{
			get; protected set;
		}

		#endregion

		#region Unity Messages

		protected override void Awake()
		{
			base.Awake();

			if (StatSchema == null)
			{
				Debug.LogError(
					$"{gameObject.name} is missing stat schema for TDRSEntity component."
				);
			}

			SocialRules = new SocialRuleManager();
			IncomingRelationships = new Dictionary<SocialAgent, SocialRelationship>();
			OutgoingRelationships = new Dictionary<SocialAgent, SocialRelationship>();
		}

		protected void OnEnable()
		{
			Stats.OnValueChanged += HandleStatChanged;
		}

		protected void OnDisable()
		{
			Stats.OnValueChanged -= HandleStatChanged;
		}

		protected override void Start()
		{
			base.Start();
			Engine.RegisterAgent(this);
		}

		#endregion

		#region Public Methods

		public override void AddTrait(string traitID, int duration = -1)
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

			// Propagate on the event to a Unity event
			if (OnTraitAdded != null) OnTraitAdded.Invoke(traitID);
		}

		public override void RemoveTrait(string traitID)
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

			if (OnTraitRemoved != null) OnTraitRemoved.Invoke(traitID);
		}

		#endregion

		#region Event Handlers

		private void HandleStatChanged(object stats, (string, float) nameAndValue)
		{
			string statName = nameAndValue.Item1;
			float value = nameAndValue.Item2;
			Engine.DB.Insert($"{UID}.stat.{statName}!{value}");
			if (OnStatChange != null) OnStatChange.Invoke(statName, value);
		}

		#endregion
	}
}
