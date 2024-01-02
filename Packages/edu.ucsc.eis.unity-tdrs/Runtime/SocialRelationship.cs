using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// Represents a social connection between two SocialAgents. It manages stats, and traits
	/// associated with the relationship.
	/// </summary>
	public class SocialRelationship : SocialEntity
	{
		#region Properties

		/// <summary>
		/// Reference to the owner of the relationship
		/// </summary>
		[field: SerializeField]
		public SocialAgent Owner { get; set; }

		/// <summary>
		/// Reference to the target of the relationship
		/// </summary>
		[field: SerializeField]
		public SocialAgent Target { get; set; }

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
			Traits.OnTraitAdded += HandleTraitAdded;
			Traits.OnTraitRemoved += HandleTraitRemoved;
			Stats.OnValueChanged += HandleStatChanged;
		}

		protected void OnDisable()
		{
			Traits.OnTraitAdded -= HandleTraitAdded;
			Traits.OnTraitRemoved -= HandleTraitRemoved;
			Stats.OnValueChanged -= HandleStatChanged;
		}

		protected override void Start()
		{
			base.Start();
			Engine.RegisterRelationship(this);
		}

		#endregion

		#region Event Handlers

		private void HandleTraitAdded(object traits, string traitID)
		{
			Engine.DB.Insert($"{Owner.UID}.relationship.{Target.UID}.trait.{traitID}");
			if (OnTraitAdded != null) OnTraitAdded.Invoke(traitID);
		}

		private void HandleTraitRemoved(object traits, string traitID)
		{
			Engine.DB.Delete($"{Owner.UID}.relationship.{Target.UID}.trait.{traitID}");
			if (OnTraitRemoved != null) OnTraitRemoved.Invoke(traitID);
		}

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
