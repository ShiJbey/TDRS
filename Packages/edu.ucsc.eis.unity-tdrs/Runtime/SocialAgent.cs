using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// A user-facing Unity component for associating a GameObject with as node within
	/// the TDRS Manager's social graph.
	/// </summary>
	public class SocialAgent : SocialEntity
	{
		#region Properties

		/// <summary>
		/// Get the unique ID of the entity
		/// </summary>
		[field: SerializeField]
		public string UID { get; protected set; }

		/// <summary>
		/// Relationships directed toward this entity
		/// </summary>
		public Dictionary<SocialAgent, SocialRelationship> IncomingRelationships { get; protected set; }

		/// <summary>
		/// Relationships from this entity directed toward other entities
		/// </summary>
		public Dictionary<SocialAgent, SocialRelationship> OutgoingRelationships { get; protected set; }

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

			IncomingRelationships = new Dictionary<SocialAgent, SocialRelationship>();
			OutgoingRelationships = new Dictionary<SocialAgent, SocialRelationship>();
		}

		protected void OnEnable()
		{
			Traits.OnTraitAdded += HandleTraitAdded;
			Traits.OnTraitRemoved += HandleTraitRemoved;
			Stats.OnValueChanged += HandleStatChanged;
			SocialRules.OnRuleAdded += HandleSocialRuleAdded;
			SocialRules.OnRuleRemoved += HandleSocialRuleRemoved;
		}

		protected void OnDisable()
		{
			Traits.OnTraitAdded -= HandleTraitAdded;
			Traits.OnTraitRemoved -= HandleTraitRemoved;
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

		#region Event Handlers

		private void HandleTraitAdded(object traits, string traitID)
		{
			Engine.DB.Insert($"{UID}.trait.{traitID}");
			if (OnTraitAdded != null) OnTraitAdded.Invoke(traitID);
		}

		private void HandleTraitRemoved(object traits, string traitID)
		{
			Engine.DB.Delete($"{UID}.trait.{traitID}");
			if (OnTraitRemoved != null) OnTraitRemoved.Invoke(traitID);
		}

		private void HandleStatChanged(object stats, (string, float) nameAndValue)
		{
			string statName = nameAndValue.Item1;
			float value = nameAndValue.Item2;
			Engine.DB.Insert($"{UID}.stat.{statName}!{value}");
			if (OnStatChange != null) OnStatChange.Invoke(statName, value);
		}

		private void HandleSocialRuleAdded(object socialRules, SocialRule rule)
		{
			Dictionary<SocialAgent, SocialRelationship> relationships;

			if (rule.IsOutgoing)
			{
				relationships = OutgoingRelationships;
			}
			else
			{
				relationships = IncomingRelationships;
			}

			foreach (var (_, relationship) in relationships)
			{
				if (rule.CheckPreconditions(relationship))
				{
					relationship.SocialRules.AddSocialRule(rule);
					rule.OnAdd(relationship);
				}
			}
		}

		private void HandleSocialRuleRemoved(object socialRules, SocialRule rule)
		{
			Dictionary<SocialAgent, SocialRelationship> relationships;
			if (rule.IsOutgoing)
			{
				relationships = OutgoingRelationships;
			}
			else
			{
				relationships = IncomingRelationships;
			}

			foreach (var (_, relationship) in relationships)
			{
				if (relationship.SocialRules.HasSocialRule(rule))
				{
					rule.OnRemove(relationship);
					relationship.SocialRules.RemoveSocialRule(rule);
				}
			}
		}

		#endregion
	}
}
