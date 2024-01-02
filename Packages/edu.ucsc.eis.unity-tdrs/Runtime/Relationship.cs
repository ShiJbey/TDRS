using System;
using UnityEngine;
using UnityEngine.Events;

namespace TDRS
{
	public class Relationship : MonoBehaviour
	{
		[Serializable]
		public struct StatInitializer
		{
			public string name;
			public float baseValue;
		}

		[SerializeField]
		public TDRSEntity owner;

		[SerializeField]
		public TDRSEntity target;

		[SerializeField]
		public StatSchemaScriptableObj statSchema;

		[SerializeField]
		public string[] traitsAtStart;

		[SerializeField]
		public StatInitializer[] baseStats;

		public TDRSEntity Owner => owner;

		public TDRSEntity Target => target;

		public TraitAddedEvent OnTraitAdded;

		public TraitRemovedEvent OnTraitRemoved;

		public StatChangeEvent OnStatChange;

		void Awake()
		{
			if (statSchema == null)
			{
				Debug.LogError(
					$"{gameObject.name} is missing stat schema for TDRSEntity component."
				);
			}
		}

		void Start()
		{
			TDRSManager manager = FindObjectOfType<TDRSManager>();
			var relationship = manager.RegisterRelationship(this);

			relationship.Traits.OnTraitAdded += (traits, traitID) =>
			{
				if (OnTraitAdded != null) OnTraitAdded.Invoke(traitID);
			};

			relationship.Traits.OnTraitRemoved += (traits, traitID) =>
			{
				if (OnTraitRemoved != null) OnTraitRemoved.Invoke(traitID);
			};

			relationship.Stats.OnValueChanged += (stats, nameAndValue) =>
			{
				string statName = nameAndValue.Item1;
				float value = nameAndValue.Item2;
				if (OnStatChange != null) OnStatChange.Invoke(statName, value);
			};
		}

		#region Custom Event Classes

		/// <summary>
		/// Event dispatched when a trait is added to a social entity
		/// </summary>
		[System.Serializable]
		public class TraitAddedEvent : UnityEvent<string> { }

		/// <summary>
		/// Event dispatched when a trait is removed from a social entity
		/// </summary>
		[System.Serializable]
		public class TraitRemovedEvent : UnityEvent<string> { }

		/// <summary>
		/// Event dispatched when a social entity has a stat that is changed
		/// </summary>
		[System.Serializable]
		public class StatChangeEvent : UnityEvent<string, float> { }

		#endregion
	}

}
