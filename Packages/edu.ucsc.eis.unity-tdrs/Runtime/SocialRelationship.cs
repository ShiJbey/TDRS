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
					$"{gameObject.name} is missing stat schema for TDRSEntity component."
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
			Trait trait = Engine.TraitLibrary.CreateInstance(traitID, this);
			Traits.AddTrait(trait, duration);

			// Apply the trait's effects on the owner
			foreach (var effect in trait.Effects)
			{
				effect.Apply();
			}

			// Add the social rules for this trait
			foreach (var socialRuleDef in trait.SocialRuleDefinitions)
			{
				// SocialRules.AddSocialRuleDefinition(socialRuleDef, trait);
			}

			// Propagate on the event to a Unity event
			Engine.DB.Insert($"{Owner.UID}.relationship.{Target.UID}.trait.{traitID}");
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

			Engine.DB.Delete($"{Owner.UID}.relationship.{Target.UID}.trait.{traitID}");
			if (OnTraitRemoved != null) OnTraitRemoved.Invoke(traitID);
		}

		#endregion


		#region Event Handlers

		private void HandleStatChanged(object stats, (string, float) nameAndValue)
		{
			string statName = nameAndValue.Item1;
			float value = nameAndValue.Item2;
			Engine.DB.Insert($"{Owner.UID}.relationship.{Target.UID}.stat.{statName}!{value}");
			if (OnStatChange != null) OnStatChange.Invoke(statName, value);
		}

		#endregion
	}

}
