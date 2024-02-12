using System;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEngine;
using UnityEngine.Events;

namespace TDRS
{
	/// <summary>
	/// Represents a social connection between two SocialAgents. It manages stats, and traits
	/// associated with the relationship.
	/// </summary>
	public class RelationshipController : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private AgentController m_owner;

		[SerializeField]
		private AgentController m_target;

		[SerializeField]
		private List<StatInitializer> m_baseStats;

		[SerializeField]
		private List<string> m_baseTraits;

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
		/// Event invoked when an entity is ticked
		/// </summary>
		public TickEvent OnTick;

		/// <summary>
		/// Event invoked when this object is registered with the social engine.
		/// </summary>
		public RegisteredEvent OnRegistered;

		#endregion

		#region Properties

		/// <summary>
		/// Reference to the owner of the relationship
		/// </summary>
		public AgentController Owner => m_owner;

		/// <summary>
		/// Reference to the target of the relationship
		/// </summary>
		public AgentController Target => m_target;

		public Relationship Edge { get; private set; }

		/// <summary>
		/// Initial values for this entity's stats.
		/// </summary>
		public List<StatInitializer> BaseStats => m_baseStats;

		/// <summary>
		/// IDs of traits to add when initializing the entity.
		/// </summary>
		public List<string> BaseTraits => m_baseTraits;

		#endregion

		#region Unity Lifecycle Methods

		private void OnEnable()
		{
			if (Edge != null)
			{
				Edge.OnTick += HandleOnTick;
				Edge.Traits.OnTraitAdded += HandleTraitAdded;
				Edge.Traits.OnTraitRemoved += HandleTraitRemoved;
				Edge.Stats.OnValueChanged += HandleStatChange;
			}
		}

		private void OnDisable()
		{
			if (Edge != null)
			{
				Edge.OnTick -= HandleOnTick;
				Edge.Traits.OnTraitAdded -= HandleTraitAdded;
				Edge.Traits.OnTraitRemoved -= HandleTraitRemoved;
				Edge.Stats.OnValueChanged -= HandleStatChange;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Set the relationship's edge reference.
		/// </summary>
		/// <param name="edge"></param>
		/// <exception cref="Exception">If edge is already set.</exception>
		public void SetEdge(Relationship edge)
		{
			if (Edge != null)
			{
				throw new Exception(
					$"Node already assigned for: ({Owner.UID},{Target.UID})");
			}

			Edge = edge;
			Edge.OnTick += HandleOnTick;
			Edge.Traits.OnTraitAdded += HandleTraitAdded;
			Edge.Traits.OnTraitRemoved += HandleTraitRemoved;
			Edge.Stats.OnValueChanged += HandleStatChange;
			OnRegistered?.Invoke();
		}

		#endregion

		#region Private Methods

		private void HandleOnTick(object sender, EventArgs args)
		{
			OnTick?.Invoke();
		}

		private void HandleStatChange(object sender, StatManager.OnValueChangedArgs args)
		{
			OnStatChange?.Invoke(args.StatName, args.Value);
		}

		private void HandleTraitAdded(object sender, TraitManager.OnTraitAddedArgs args)
		{
			OnTraitAdded?.Invoke(args.Trait.TraitID);
		}

		private void HandleTraitRemoved(object sender, TraitManager.OnTraitRemovedArgs args)
		{
			OnTraitRemoved?.Invoke(args.Trait.TraitID);
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

		/// <summary>
		/// Event dispatched when this object is registered with the social engine
		/// </summary>
		[System.Serializable]
		public class RegisteredEvent : UnityEvent { }

		#endregion
	}

}
