using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


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
	/// This is a singleton class. Only one TDRSManager should be present in a scene.
	/// </para>
	/// </summary>
	[DefaultExecutionOrder(-5)]
	public class TDRSManager : MonoBehaviour
	{
		#region Attributes

		/// <summary>
		/// A list of TextAssets assigned within the Unity inspector
		/// </summary>
		[SerializeField]
		protected List<TextAsset> _traitDefinitions = new List<TextAsset>();

		protected Queue<Relationship> relationshipQueue;

		public LoadFactoriesEvent OnLoadFactories;

		#endregion

		#region Properties

		public static TDRSManager Instance { get; private set; }
		public SocialEngine SocialEngine { get; } = new SocialEngine();

		#endregion

		#region Unity Methods
		private void Awake()
		{
			// Ensure there is only one instance of this MonoBehavior active within the scene
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
				relationshipQueue = new Queue<Relationship>();
			}
		}

		void Start()
		{
			LoadFactories();
			LoadTraits();
		}
		#endregion

		#region Content Loading Methods

		private void LoadFactories()
		{
			OnLoadFactories.Invoke(SocialEngine);
		}

		private void LoadTraits()
		{
			foreach (var textAsset in _traitDefinitions)
			{
				SocialEngine.TraitLibrary.LoadTraits(textAsset.text);
			}

			SocialEngine.TraitLibrary.InstantiateTraits(SocialEngine);
		}

		#endregion

		/// <summary>
		/// Register a new entity with the manager.
		/// </summary>
		/// <param name="entity"></param>
		public TDRSNode RegisterEntity(TDRSEntity entity)
		{
			var node = new TDRSNode(SocialEngine, entity.entityID, entity.gameObject);
			SocialEngine.AddNode(node);

			foreach (var entry in entity.StatSchema.stats)
			{
				node.Stats.AddStat(
					entry.statName,
					new StatSystem.Stat(
						entry.baseValue, entry.minValue, entry.maxValue, entry.isDiscrete
					)
				);
			}

			// Configure initial traits
			foreach (var traitID in entity.traitsAtStart)
			{
				node.AddTrait(traitID);
			}

			// Configure initial stats
			foreach (var entry in entity.baseStats)
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
		public TDRSRelationship RegisterRelationship(Relationship relationship)
		{
			if (!SocialEngine.HasNode(relationship.Owner.entityID))
			{
				relationshipQueue.Enqueue(relationship);
				return null;
			}

			if (!SocialEngine.HasNode(relationship.Target.entityID))
			{
				relationshipQueue.Enqueue(relationship);
				return null;
			}

			var owner = SocialEngine.GetNode(relationship.Owner.entityID);
			var target = SocialEngine.GetNode(relationship.Target.entityID);

			var rel = new TDRSRelationship(
				SocialEngine,
				$"{relationship.Owner.entityID}=>{relationship.Target.entityID}",
				owner,
				target,
				relationship.gameObject
			);

			foreach (var entry in relationship.statSchema.stats)
			{
				rel.Stats.AddStat(
					entry.statName,
					new StatSystem.Stat(
						entry.baseValue, entry.minValue, entry.maxValue, entry.isDiscrete
					)
				);
			}

			// Configure initial traits
			foreach (var traitID in relationship.traitsAtStart)
			{
				rel.AddTrait(traitID);
			}

			// Configure initial stats
			foreach (var entry in relationship.baseStats)
			{
				rel.Stats.GetStat(entry.name).BaseValue = entry.baseValue;
			}

			SocialEngine.AddRelationship(rel);

			return rel;
		}

		private void Update()
		{
			ProcessRelationshipQueue();
		}

		private void ProcessRelationshipQueue()
		{
			List<Relationship> relationships = new List<Relationship>(relationshipQueue);
			relationshipQueue.Clear();

			foreach (var relationship in relationships)
			{
				RegisterRelationship(relationship);
			}
		}
	}


	#region Custom Event Classes

	[Serializable]
	/// <summary>
	/// Event dispatched when the TDRSmanager is loading factory instances during start.
	/// </summary>
	public class LoadFactoriesEvent : UnityEvent<SocialEngine> { }

	#endregion
}
