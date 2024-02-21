using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

namespace TDRS
{
	/// <summary>
	/// A user-facing Unity component for associating a GameObject with an agent within
	/// the social engine's social network
	/// </summary>
	public class AgentController : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private string m_UID;

		[SerializeField]
		private AgentSchemaSO m_agentSchema;

		[SerializeField]
		private List<StatInitializer> m_baseStats;

		[SerializeField]
		private List<string> m_baseTraits;

		#endregion

		#region Properties

		/// <summary>
		/// Get the unique ID of the entity
		/// </summary>
		public string UID => m_UID;

		/// <summary>
		/// A reference to the configuration settings for this agent
		/// </summary>
		public AgentSchemaSO Schema => m_agentSchema;

		/// <summary>
		/// A reference to this controller's corresponding agent within the social engine.
		/// </summary>
		public Agent Agent { get; private set; }

		/// <summary>
		/// Initial values for this entity's stats.
		/// </summary>
		public List<StatInitializer> BaseStats => m_baseStats;

		/// <summary>
		/// IDs of traits to add when initializing the entity.
		/// </summary>
		public List<string> BaseTraits => m_baseTraits;

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

		#region Unity Messages

		private void OnEnable()
		{
			if (Agent != null)
			{
				Agent.OnTick += HandleOnTick;
				Agent.OnTraitAdded += HandleTraitAdded;
				Agent.OnTraitRemoved += HandleTraitRemoved;
				Agent.OnStatChanged += HandleStatChange;
			}
		}

		private void OnDisable()
		{
			if (Agent != null)
			{
				Agent.OnTick -= HandleOnTick;
				Agent.OnTraitAdded -= HandleTraitAdded;
				Agent.OnTraitRemoved -= HandleTraitRemoved;
				Agent.OnStatChanged -= HandleStatChange;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Set the controller's agent reference.
		/// </summary>
		/// <param name="agent"></param>
		/// <exception cref="Exception">If agent is already set.</exception>
		public void SetAgent(Agent agent)
		{
			if (Agent != null) throw new Exception($"Agent already assigned for: {UID}");

			Agent = agent;
			Agent.OnTick += HandleOnTick;
			Agent.OnTraitAdded += HandleTraitAdded;
			Agent.OnTraitRemoved += HandleTraitRemoved;
			Agent.OnStatChanged += HandleStatChange;
			OnRegistered?.Invoke();
		}

		#endregion

		#region Private Methods

		private void HandleOnTick(object sender, EventArgs args)
		{
			OnTick?.Invoke();
		}

		private void HandleStatChange(object sender, Agent.OnStatChangedArgs args)
		{
			OnStatChange?.Invoke(args.StatName, args.Value);
		}

		private void HandleTraitAdded(object sender, Agent.OnTraitAddedArgs args)
		{
			OnTraitAdded?.Invoke(args.Trait.TraitID);
		}

		private void HandleTraitRemoved(object sender, Agent.OnTraitRemovedArgs args)
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
		/// Event dispatched when a stat is changed
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
