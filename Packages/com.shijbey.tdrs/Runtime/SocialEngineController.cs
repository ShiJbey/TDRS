using UnityEngine;
using RePraxis;
using UnityEngine.Events;
using TDRS.Serialization;

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
	public class SocialEngineController : MonoBehaviour
	{
		#region Fields

		/// <summary>
		/// ScriptableObjects containing settings for constructing new agents
		/// in the social graph.
		/// </summary>
		[SerializeField]
		private AgentSchemaSO[] m_agentSchemas;

		/// <summary>
		/// ScriptableObjects containing settings for constructing new relationships
		/// in the social graph.
		/// </summary>
		[SerializeField]
		private RelationshipSchemaSO[] m_relationshipSchemas;

		/// <summary>
		/// A list of text files containing social event definitions.
		/// </summary>
		[SerializeField]
		private SocialEventSO[] m_socialEvents;

		/// <summary>
		/// A list of text files containing social event definitions.
		/// </summary>
		[SerializeField]
		private SocialRuleSO[] m_socialRules;

		/// <summary>
		/// ScriptableObject trait definitions
		/// </summary>
		[SerializeField]
		private TraitSO[] m_traits;

		/// <summary>
		/// Should the social engine not be destroyed when loading a new scene.
		/// <para>
		/// If you're not doing additive scene loading, then this should probably be set to true.
		/// </para>
		/// </summary>
		[SerializeField]
		private bool m_dontDestroyOnLoad;

		/// <summary>
		/// The social engine that the controller manages.
		/// </summary>
		private SocialEngine _state;

		#endregion

		#region Properties

		public SocialEngine State
		{
			get { return _state; }
			private set
			{
				if (_state != null)
				{
					_state.OnAgentAdded -= HandleAgentAdded;
					_state.OnAgentRemoved -= HandleAgentRemoved;
					_state.OnRelationshipAdded -= HandleRelationshipAdded;
					_state.OnRelationshipRemoved -= HandleRelationshipRemoved;
				}

				_state = value;
				_state.OnAgentAdded += HandleAgentAdded;
				_state.OnAgentRemoved += HandleAgentRemoved;
				_state.OnRelationshipAdded += HandleRelationshipAdded;
				_state.OnRelationshipRemoved += HandleRelationshipRemoved;
			}
		}
		public static SocialEngineController Instance { get; private set; }
		public RePraxisDatabase DB => State.DB;

		#endregion

		#region Events and Actions

		public static UnityAction<SocialEngine> OnLoadTraits;

		public static UnityAction<SocialEngine> OnLoadAgentSchemas;

		public static UnityAction<SocialEngine> OnLoadRelationshipSchemas;

		public static UnityAction<SocialEngine> OnLoadSocialRules;

		public static UnityAction<SocialEngine> OnLoadSocialEvents;

		public static UnityAction<SocialEngine> OnRegisterEffectFactories;

		public static UnityAction<SocialEngine> OnRegisterAgentsAndRelationships;

		public static UnityAction<Agent> OnAgentAdded;

		public static UnityAction<Relationship> OnRelationshipAdded;

		public static UnityAction<Agent> OnAgentRemoved;

		public static UnityAction<Relationship> OnRelationshipRemoved;

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
				State = SocialEngine.Instantiate(null);

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
			LoadAgentSchemas();
			LoadRelationshipSchemas();
			LoadSocialRules();
			LoadSocialEvents();
			RegisterEffectFactories();
		}

		/// <summary>
		/// Register a new entity with the manager.
		/// </summary>
		/// <param name="agentController"></param>
		public Agent RegisterAgent(AgentController agentController)
		{
			Agent agent;

			if (State.HasAgent(agentController.UID))
			{
				agent = State.GetAgent(agentController.UID);
			}
			else
			{
				agent = State.AddAgent(agentController.Schema.agentType, agentController.UID);

				// Configure initial traits
				foreach (var traitID in agentController.BaseTraits)
				{
					agent.AddTrait(traitID);
				}

				// Configure initial stats
				foreach (var entry in agentController.BaseStats)
				{
					agent.Stats.GetStat(entry.name).BaseValue = entry.baseValue;
				}
			}

			agentController.SetAgent(agent);

			return agent;
		}

		/// <summary>
		/// Register a new relationship with the manager.
		/// </summary>
		/// <param name="relationshipController"></param>
		/// <returns></returns>
		public Relationship RegisterRelationship(RelationshipController relationshipController)
		{
			Relationship relationship;

			if (State.HasRelationship(relationshipController.Owner.UID, relationshipController.Target.UID))
			{
				relationship = State.GetRelationship(relationshipController.Owner.UID, relationshipController.Target.UID);
			}
			else
			{
				if (!State.HasAgent(relationshipController.Owner.UID))
				{
					RegisterAgent(relationshipController.Owner);
				}

				if (!State.HasAgent(relationshipController.Target.UID))
				{
					RegisterAgent(relationshipController.Target);
				}

				relationship = State.AddRelationship(
					relationshipController.Owner.UID,
					relationshipController.Target.UID);


				// Configure initial stats
				foreach (var entry in relationshipController.BaseStats)
				{
					relationship.Stats.GetStat(entry.name).BaseValue = entry.baseValue;
				}

				if (relationshipController.BaseRelationshipType != "")
				{
					relationship.SetRelationshipType(relationshipController.BaseRelationshipType);
				}

				// Configure initial traits
				foreach (var traitID in relationshipController.BaseTraits)
				{
					relationship.AddTrait(traitID);
				}
			}

			relationshipController.SetRelationship(relationship);

			return relationship;
		}

		/// <summary>
		/// Register all agent and relationship GameObjects in the
		/// </summary>
		public void RegisterAgentsAndRelationships()
		{
			RegisterSocialAgentsInScene();
			RegisterSocialRelationshipsInScene();
			OnRegisterAgentsAndRelationships?.Invoke(State);
		}

		#endregion

		#region Private Methods

		private void RegisterSocialAgentsInScene()
		{
			var socialAgents = FindObjectsOfType<AgentController>();
			foreach (AgentController agent in socialAgents)
			{
				RegisterAgent(agent);
			}
		}

		private void RegisterSocialRelationshipsInScene()
		{
			var relationships = FindObjectsOfType<RelationshipController>();
			foreach (RelationshipController relationship in relationships)
			{
				RegisterRelationship(relationship);
			}
		}

		private void LoadAgentSchemas()
		{
			foreach (AgentSchemaSO schemaSO in m_agentSchemas)
			{
				var schema = schemaSO.CreateAgentSchema();
				State.AddAgentSchema(schema);
			}

			OnLoadAgentSchemas?.Invoke(State);
		}

		private void LoadRelationshipSchemas()
		{
			foreach (RelationshipSchemaSO schemaSO in m_relationshipSchemas)
			{
				var schema = schemaSO.CreateRelationshipSchema();
				State.AddRelationshipSchema(schema);
			}

			OnLoadRelationshipSchemas?.Invoke(State);
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

		private void LoadSocialRules()
		{
			foreach (SocialRuleSO ruleSO in m_socialRules)
			{
				State.AddSocialRule(ruleSO.ToRuntimeInstance());
			}

			OnLoadSocialRules?.Invoke(State);
		}

		private void LoadTraits()
		{
			foreach (TraitSO traitSO in m_traits)
			{
				Trait trait = traitSO.CreateTrait();
				State.TraitLibrary.Traits[trait.TraitID] = trait;
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


		private void HandleAgentAdded(object obj, SocialEngine.OnAgentAddedArgs args)
		{
			OnAgentAdded?.Invoke(args.Agent);
		}

		private void HandleAgentRemoved(object obj, SocialEngine.OnAgentRemovedArgs args)
		{
			OnAgentRemoved?.Invoke(args.Agent);
		}

		private void HandleRelationshipAdded(object obj, SocialEngine.OnRelationshipAddedArgs args)
		{
			OnRelationshipAdded?.Invoke(args.Relationship);
		}

		private void HandleRelationshipRemoved(object obj, SocialEngine.OnRelationshipRemovedArgs args)
		{
			OnRelationshipRemoved?.Invoke(args.Relationship);
		}

		#endregion
	}
}
