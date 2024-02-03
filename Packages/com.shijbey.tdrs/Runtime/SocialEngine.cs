using UnityEngine;
using RePraxis;
using UnityEngine.Events;
using TDRS.StatSystem;

namespace TDRS
{
	/// <summary>
	/// This is the main entry point and overall manager for all information related
	/// to the Trait-Driven Relationship System.
	///
	/// <para>
	/// This MonoBehaviour is responsible for managing information about all the
	/// traits, preconditions, and effects.
	/// </para>
	///
	/// <para>
	/// This is a singleton class. Only one SocialEngine should be present in a scene.
	/// </para>
	/// </summary>
	public class SocialEngine : MonoBehaviour
	{
		#region Fields

		/// <summary>
		/// ScriptableObjects containing settings for constructing new nodes
		/// in the social graph.
		/// </summary>
		[SerializeField]
		private AgentConfigSO[] m_agentConfigs;

		/// <summary>
		/// ScriptableObjects containing settings for constructing new relationships
		/// in the social graph.
		/// </summary>
		[SerializeField]
		private RelationshipConfigSO[] m_relationshipConfigs;

		/// <summary>
		/// A list of text files containing social event definitions.
		/// </summary>
		[SerializeField]
		private SocialEventSO[] m_socialEvents;

		/// <summary>
		/// ScriptableObject trait definitions
		/// </summary>
		[SerializeField]
		protected TraitSO[] m_traits;

		/// <summary>
		/// Should the social engine not be destroyed when loading a new scene.
		/// <para>
		/// If you're not doing additive scene loading, then this should probably be set to true.
		/// </para>
		/// </summary>
		[SerializeField]
		private bool m_dontDestroyOnLoad;

		#endregion

		#region Properties

		public SocialEngineState State { get; private set; }
		public static SocialEngine Instance { get; private set; }
		public RePraxisDatabase DB => State.DB;

		#endregion

		#region Events and Actions

		public static UnityAction<SocialEngineState> OnLoadTraits;

		public static UnityAction<SocialEngineState> OnLoadAgentConfigs;

		public static UnityAction<SocialEngineState> OnLoadRelationshipConfigs;

		public static UnityAction<SocialEngineState> OnLoadSocialEvents;

		public static UnityAction<SocialEngineState> OnRegisterEffectFactories;

		#endregion

		#region Unity Messages

		private void Awake()
		{
			// Ensure there is only one instance of this MonoBehavior active within the scene
			if (Instance != null && Instance != this)
			{
				Debug.LogError(
					"Only on SocialEngine may be active in a scene. Destroying this one."
				);
				Destroy(this);
			}
			else
			{
				Instance = this;
				State = SocialEngineState.CreateState(null);

				if (m_dontDestroyOnLoad)
				{
					DontDestroyOnLoad(this);
				}
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// This function should be called by a games "GameManager" or related class when it wants
		/// the social engine to load all configuration data and register information from
		/// agents and relationships defined in the editor.
		/// </summary>
		public void Initialize()
		{
			LoadTraits();
			LoadAgentConfigs();
			LoadRelationshipConfigs();
			LoadSocialEvents();
			RegisterEffectFactories();
			RegisterSocialAgentsInScene();
			RegisterSocialRelationshipsInScene();
		}

		/// <summary>
		/// Register a new entity with the manager.
		/// </summary>
		/// <param name="agent"></param>
		public AgentNode RegisterAgent(SocialAgent agent)
		{
			if (State.HasAgent(agent.UID)) return State.GetAgent(agent.UID);

			AgentNode node = State.AddAgent(agent.Config.agentType, agent.UID);

			agent.SetNode(node);

			// Configure initial traits
			foreach (var traitID in agent.BaseTraits)
			{
				node.AddTrait(traitID);
			}

			// Configure initial stats
			foreach (var entry in agent.BaseStats)
			{
				node.Stats.GetStat(entry.name).BaseValue = entry.baseValue;
			}

			return node;
		}

		/// <summary>
		/// Register a new relationship with the manager.
		/// </summary>
		/// <param name="relationship"></param>
		/// <returns></returns>
		public RelationshipEdge RegisterRelationship(SocialRelationship relationship)
		{
			if (State.HasRelationship(relationship.Owner.UID, relationship.Target.UID))
			{
				return State.GetRelationship(relationship.Owner.UID, relationship.Target.UID);
			}

			if (!State.HasAgent(relationship.Owner.UID))
			{
				RegisterAgent(relationship.Owner);
			}

			if (!State.HasAgent(relationship.Target.UID))
			{
				RegisterAgent(relationship.Target);
			}

			RelationshipEdge relationshipEdge = State.AddRelationship(
				relationship.Owner.UID,
				relationship.Target.UID);

			relationship.SetEdge(relationshipEdge);

			// Configure initial stats
			foreach (var entry in relationship.BaseStats)
			{
				relationshipEdge.Stats.GetStat(entry.name).BaseValue = entry.baseValue;
			}

			// Configure initial traits
			foreach (var traitID in relationship.BaseTraits)
			{
				relationshipEdge.AddTrait(traitID);
			}

			return relationshipEdge;
		}

		#endregion

		#region Private Methods

		private void RegisterSocialAgentsInScene()
		{
			var socialAgents = FindObjectsOfType<SocialAgent>();
			foreach (SocialAgent agent in socialAgents)
			{
				RegisterAgent(agent);
			}
		}

		private void RegisterSocialRelationshipsInScene()
		{
			var relationships = FindObjectsOfType<SocialRelationship>();
			foreach (SocialRelationship relationship in relationships)
			{
				RegisterRelationship(relationship);
			}
		}

		private void LoadAgentConfigs()
		{
			foreach (AgentConfigSO configSO in m_agentConfigs)
			{
				State.AddAgentConfig(configSO.CreateAgentConfig());
			}

			OnLoadAgentConfigs?.Invoke(State);
		}

		private void LoadRelationshipConfigs()
		{
			foreach (RelationshipConfigSO configSO in m_relationshipConfigs)
			{
				State.AddRelationshipConfig(configSO.CreateRelationshipConfig());
			}

			OnLoadRelationshipConfigs?.Invoke(State);
		}

		private void LoadSocialEvents()
		{
			foreach (SocialEventSO socialEventDef in m_socialEvents)
			{
				State.SocialEventLibrary.AddSocialEvent(
					socialEventDef.GetSocialEvent()
				);
			}

			OnLoadSocialEvents?.Invoke(State);
		}

		private void LoadTraits()
		{
			foreach (TraitSO traitDef in m_traits)
			{
				State.TraitLibrary.AddTraitDefinition(traitDef.GetTraitDefinition());
			}

			OnLoadTraits?.Invoke(State);
		}

		private void RegisterEffectFactories()
		{
			var effectFactories = GetComponents<EffectFactory>();
			foreach (var factory in effectFactories)
			{
				State.EffectLibrary.AddEffectFactory(factory);
			}

			OnRegisterEffectFactories?.Invoke(State);
		}

		#endregion
	}
}
