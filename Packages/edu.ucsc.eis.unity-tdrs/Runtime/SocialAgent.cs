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

		#region Unity Methods

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
			SocialRules.OnRuleAdded += HandleSocialRuleAdded;
			SocialRules.OnRuleRemoved += HandleSocialRuleRemoved;
		}

		protected void OnDisable()
		{
			Stats.OnValueChanged -= HandleStatChanged;
			SocialRules.OnRuleAdded -= HandleSocialRuleAdded;
			SocialRules.OnRuleRemoved -= HandleSocialRuleRemoved;
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
			Trait trait = Engine.TraitLibrary.CreateInstance(traitID, this);
			Traits.AddTrait(trait, duration);

			// Apply the trait's effects on the owner
			foreach (var effect in trait.Effects)
			{
				effect.Apply();
			}

			Engine.DB.Insert($"{UID}.trait.{traitID}");

			// Add the social rules for this trait
			foreach (var socialRuleDef in trait.SocialRuleDefinitions)
			{
				// SocialRules.AddSocialRuleDefinition(socialRuleDef, trait);
			}

			// Propagate on the event to a Unity event
			if (OnTraitAdded != null) OnTraitAdded.Invoke(traitID);
		}

		public override void RemoveTrait(string traitID)
		{
			var trait = Traits.GetTrait(traitID);

			// Undo the effects of the trait on the owner
			foreach (var effect in trait.Effects)
			{
				effect.Remove();
			}

			Traits.RemoveTrait(trait);

			Engine.DB.Delete($"{UID}.trait.{traitID}");
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


		private void HandleSocialRuleAdded(object socialRules, SocialRuleDefinition socialRule)
		{
			// Try to apply the social rules to existing outgoing relationships
			foreach (var (target, relationship) in OutgoingRelationships)
			{
				var bindings = new Dictionary<string, string>()
				{
					{"?owner", UID},
					{"?other", target.UID}
				};

				var results = socialRule.Query.Run(Engine.DB, bindings);

				if (!results.Success) continue;

				foreach (var result in results.Bindings)
				{

				}
			}

			// Try to apply the social rule to existing incoming relationships
			foreach (var (owner, relationship) in IncomingRelationships)
			{
				var bindings = new Dictionary<string, string>()
				{
					{"?owner", UID},
					{"?other", owner.UID}
				};

				var results = socialRule.Query.Run(Engine.DB, bindings);

				if (!results.Success) continue;

				foreach (var result in results.Bindings)
				{

				}
			}
		}



		private void HandleSocialRuleRemoved(object socialRules, SocialRuleDefinition rule)
		{
			// Remove all social rule instances made from the rule
		}

		#endregion
	}
}
