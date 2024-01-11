using System.Collections.Generic;
using TDRS.StatSystem;
using UnityEngine;
using UnityEngine.Events;

namespace TDRS
{
	/// <summary>
	/// An object within the social engine's social network that has stats and traits associated
	/// with it.
	/// </summary>
	public abstract class SocialEntity : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private StatSchemaScriptableObj m_statSchema;

		[SerializeField]
		private List<StatInitializer> m_baseStats;

		[SerializeField]
		private List<string> m_baseTraits;

		#endregion

		#region Properties

		/// <summary>
		/// The schema of what stats to add to the entity on initialization.
		/// </summary>
		public StatSchemaScriptableObj StatSchema => m_statSchema;

		/// <summary>
		/// Initial values for this entity's stats.
		/// </summary>
		public List<StatInitializer> BaseStats => m_baseStats;

		/// <summary>
		/// IDs of traits to add when initializing the entity.
		/// </summary>
		public List<string> BaseTraits => m_baseTraits;

		/// <summary>
		/// A reference to the manager that owns this entity
		/// </summary>
		public SocialEngine Engine { get; protected set; }

		/// <summary>
		/// The collection of traits associated with this entity
		/// </summary>
		public TraitManager Traits { get; protected set; }

		/// <summary>
		/// A collection of stats associated with this entity
		/// </summary>
		public StatManager Stats { get; protected set; }

		#endregion

		#region Events

		/// <summary>
		/// Event invoked when a trait is added to the entity
		/// </summary>
		public TraitAddedEvent OnTraitAdded;

		/// <summary>
		/// Event invoked when a trait is removed from the entity
		/// </summary>
		public TraitRemovedEvent OnTraitRemoved;

		/// <summary>
		/// Event invoked when a stat value changes on the entity
		/// </summary>
		public StatChangeEvent OnStatChange;

		/// <summary>
		/// Event invoked when an entity is ticked;
		/// </summary>
		public TickEvent OnTick;

		#endregion

		#region Constructors

		protected virtual void Awake()
		{
			Traits = new TraitManager();
			Stats = new StatManager();
		}

		protected virtual void Start()
		{
			Engine = FindObjectOfType<SocialEngine>();

			if (Engine == null)
			{
				Debug.LogError("Cannot find GameObject with TDRSManager component in scene.");
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a trait to an entity.
		/// </summary>
		/// <param name="traitID"></param>
		/// <param name="duration"></param>
		public abstract void AddTrait(string traitID, int duration = -1);

		/// <summary>
		/// Remove a trait from the entity.
		/// </summary>
		/// <param name="traitID"></param>
		public abstract void RemoveTrait(string traitID);

		/// <summary>
		/// Advance the simulation by one simulation tick
		/// </summary>
		public virtual void Tick()
		{
			TickTraits();
			TickStats();

			if (OnTick != null) OnTick.Invoke();
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

		#region Custom Event Classes

		/// <summary>
		/// Event dispatched when an entity is ticked
		/// </summary>
		[System.Serializable]
		public class TickEvent : UnityEvent { }

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