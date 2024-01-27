using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TDRS
{
	/// <summary>
	/// A user-facing Unity component for associating a GameObject with as node within
	/// the social engines's social network
	/// </summary>
	public class SocialAgent : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private string m_UID;

		[SerializeField]
		private AgentConfigSO m_agentConfig;

		[SerializeField]
		private List<StatInitializer> m_baseStats;

		[SerializeField]
		private List<string> m_baseTraits;

		private AgentNode m_agentNode;

		#endregion

		#region Properties

		/// <summary>
		/// Get the unique ID of the entity
		/// </summary>
		public string UID => m_UID;

		/// <summary>
		/// A reference to the config settings for this agent
		/// </summary>
		public AgentConfigSO Config => m_agentConfig;

		/// <summary>
		/// A reference to this agent's corresponding node within the social engine.
		/// </summary>
		public AgentNode Node => m_agentNode;

		/// <summary>
		/// A reference to the manager that owns this entity
		/// </summary>
		public SocialEngine Engine { get; protected set; }

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
		/// Event invoked when an entity is ticked;
		/// </summary>
		public TickEvent OnTick;

		#endregion

		#region Unity Messages

		protected void Start()
		{
			Engine = FindObjectOfType<SocialEngine>();

			if (Engine == null)
			{
				Debug.LogError("Cannot find GameObject with SocialEngine component in scene.");
			}

			m_agentNode = Engine.RegisterAgent(this);

			// add event listeners
			m_agentNode.OnTick += HandleOnTick;
			m_agentNode.Traits.OnTraitAdded += HandleTraitAdded;
			m_agentNode.Traits.OnTraitRemoved += HandleTraitRemoved;
			m_agentNode.Stats.OnValueChanged += HandleStatChange;
		}

		private void OnDisable()
		{
			if (m_agentNode != null)
			{
				m_agentNode.OnTick -= HandleOnTick;
				m_agentNode.Traits.OnTraitAdded -= HandleTraitAdded;
				m_agentNode.Traits.OnTraitRemoved -= HandleTraitRemoved;
				m_agentNode.Stats.OnValueChanged -= HandleStatChange;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a trait to the agent
		/// </summary>
		/// <param name="traitID"></param>
		/// <param name="duration"></param>
		public void AddTrait(string traitID, int duration = -1)
		{
			if (m_agentNode == null)
			{
				Debug.LogError($"Cannot add {traitID} trait. Agent missing node reference.");
				return;
			}

			m_agentNode.AddTrait(traitID, duration);
		}

		/// <summary>
		/// Remove a trait from the Agent
		/// </summary>
		/// <param name="traitID"></param>
		public void RemoveTrait(string traitID)
		{
			if (m_agentNode == null)
			{
				Debug.LogError($"Cannot remove {traitID} trait. Agent missing node reference.");
				return;
			}

			m_agentNode.RemoveTrait(traitID);
		}

		#endregion

		#region Private Methods

		private void HandleOnTick(object sender, EventArgs args)
		{
			OnTick?.Invoke();
		}

		private void HandleStatChange(object sender, (string, float) args)
		{
			OnStatChange?.Invoke(args.Item1, args.Item2);
		}

		private void HandleTraitAdded(object sender, string trait)
		{
			OnTraitAdded?.Invoke(trait);
		}

		private void HandleTraitRemoved(object sender, string trait)
		{
			OnTraitRemoved?.Invoke(trait);
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
