using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TDRS
{
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
		public TraitCollection Traits { get; protected set; }

		/// <summary>
		/// A collection of stats associated with this entity
		/// </summary>
		public StatCollection Stats { get; protected set; }

		/// <summary>
		/// All social rules affecting this entity
		/// </summary>
		public SocialRules SocialRules { get; protected set; }

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
			Traits = new TraitCollection();
			Stats = new StatCollection();
			SocialRules = new SocialRules();
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

		#region Methods

		/// <summary>
		/// Add a trait to an entity.
		/// </summary>
		/// <param name="traitID"></param>
		public virtual void AddTrait(string traitID, int duration = -1)
		{
			var trait = Engine.TraitLibrary.GetTrait(traitID);
			Traits.AddTrait(trait);
			trait.OnAdd(this);
		}

		/// <summary>
		/// Remove a trait from the entity.
		/// </summary>
		/// <param name="traitID"></param>
		public virtual void RemoveTrait(string traitID)
		{
			var trait = Engine.TraitLibrary.GetTrait(traitID);
			Traits.RemoveTrait(trait);
			trait.OnRemove(this);
		}

		/// <summary>
		/// Advance the simulation by one simulation tick
		/// </summary>
		public void Tick()
		{
			TickTraits();
			Stats.Tick();

			if (OnTick != null) OnTick.Invoke();
		}

		private void TickTraits()
		{
			List<TraitEntry> traits = Traits.Traits;

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
					RemoveTrait(trait.Trait.TraitID);
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
