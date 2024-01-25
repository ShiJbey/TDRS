using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// Represents a social connection between two SocialAgents. It manages stats, and traits
	/// associated with the relationship.
	/// </summary>
	public class SocialRelationship : SocialEntity
	{
		#region Fields

		[SerializeField]
		private SocialAgent m_owner;

		[SerializeField]
		private SocialAgent m_target;

		#endregion

		#region Properties

		/// <summary>
		/// Reference to the owner of the relationship
		/// </summary>
		public SocialAgent Owner => m_owner;

		/// <summary>
		/// Reference to the target of the relationship
		/// </summary>
		public SocialAgent Target => m_target;

		#endregion

		#region Unity Lifecycle Methods

		protected override void Awake()
		{
			base.Awake();

			if (StatSchema == null)
			{
				Debug.LogError(
					$"{gameObject.name} is missing stat schema for SocialRelationship component."
				);
			}
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
			Engine.RegisterRelationship(this);
		}

		#endregion

		#region Public Methods

		public override void AddTrait(string traitID, int duration = -1)
		{
			if (Traits.HasTrait(traitID)) return;

			Trait trait = Engine.TraitLibrary.CreateInstance(traitID, this);
			Traits.AddTrait(trait, duration);
			Engine.DB.Insert($"{Owner.UID}.relationships.{Target.UID}.traits.{traitID}");

			// Apply the trait's effects on the owner
			foreach (var effect in trait.Effects)
			{
				effect.Apply();
			}

			// Add the social rules for this trait
			foreach (var socialRuleDef in trait.SocialRuleDefinitions)
			{
				Owner.SocialRules.AddSocialRuleDefinition(socialRuleDef);
			}

			// Propagate on the event to a Unity event
			if (OnTraitAdded != null) OnTraitAdded.Invoke(traitID);
		}

		public override void RemoveTrait(string traitID)
		{
			if (!Traits.HasTrait(traitID)) return;

			var trait = Traits.GetTrait(traitID);
			Traits.RemoveTrait(trait);
			Engine.DB.Delete($"{Owner.UID}.relationships.{Target.UID}.traits.{traitID}");

			// Undo the effects of the trait on the owner
			foreach (var effect in trait.Effects)
			{
				effect.Remove();
			}

			// Remove social rules from the relationship owner
			foreach (var socialRuleDef in trait.SocialRuleDefinitions)
			{
				Owner.SocialRules.RemoveSocialRuleDefinition(socialRuleDef);
			}

			if (OnTraitRemoved != null) OnTraitRemoved.Invoke(traitID);
		}

		#endregion


		#region Event Handlers

		private void HandleStatChanged(object stats, (string, float) nameAndValue)
		{
			string statName = nameAndValue.Item1;
			float value = nameAndValue.Item2;
			Engine.DB.Insert($"{Owner.UID}.relationships.{Target.UID}.stats.{statName}!{value}");
			if (OnStatChange != null) OnStatChange.Invoke(statName, value);
		}

		#endregion
	}

}
