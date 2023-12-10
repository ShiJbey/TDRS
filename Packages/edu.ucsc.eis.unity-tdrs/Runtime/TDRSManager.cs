using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using YamlDotNet.RepresentationModel;
using TDRS.Helpers;


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
	public class TDRSManager : MonoBehaviour
	{
		#region Helper Classes
		[Serializable]
		public struct StatConfig
		{
			public string statName;
			public float baseValue;
			public float maxValue;
			public float minValue;
			public bool isDiscrete;
		}

		#endregion

		#region Attributes

		/// <summary>
		/// All the nodes (characters, groups, concepts) in the social graph
		/// </summary>
		protected Dictionary<string, TDRSNode> _nodes = new Dictionary<string, TDRSNode>();

		[SerializeField]
		protected List<StatConfig> _entityStats;

		[SerializeField]
		protected List<StatConfig> _relationshipStats;

		/// <summary>
		/// A list of TextAssets assigned within the Unity inspector
		/// </summary>
		[SerializeField]
		protected List<TextAsset> _traitDefinitions = new List<TextAsset>();

		/// <summary>
		/// A list of precondition factories to load during wake
		/// </summary>
		[SerializeField]
		protected List<PreconditionFactorySO> _preconditionFactories = new List<PreconditionFactorySO>();

		/// <summary>
		/// A list of effect factories to load during wake
		/// </summary>
		[SerializeField]
		protected List<EffectFactorySO> _effectFactories = new List<EffectFactorySO>();

		/// <summary>
		/// A reference to a YAML initialization file for the social graph.
		/// </summary>
		[SerializeField]
		protected TextAsset _initializationFile;

		#endregion

		#region Properties

		public static TDRSManager Instance { get; private set; }
		public TraitLibrary TraitLibrary { get; private set; }
		public EffectLibrary EffectLibrary { get; private set; }
		public PreconditionLibrary PreconditionLibrary { get; private set; }

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
			}

			PreconditionLibrary = new PreconditionLibrary();
			EffectLibrary = new EffectLibrary();
			TraitLibrary = new TraitLibrary();

			LoadPreconditionFactories();
			LoadEffectFactories();
			LoadTraits();
			LoadInitializationFile();
		}
		#endregion

		#region Content Loading Methods
		private void LoadPreconditionFactories()
		{
			foreach (var factory in _preconditionFactories)
			{
				PreconditionLibrary.AddFactory(factory.preconditionType, factory);
			}
		}

		private void LoadEffectFactories()
		{
			foreach (var factory in _effectFactories)
			{
				EffectLibrary.AddFactory(factory.effectType, factory);
			}
		}

		private void LoadTraits()
		{
			foreach (var textAsset in _traitDefinitions)
			{
				TraitLibrary.LoadTraits(textAsset.text);
			}

			TraitLibrary.InstantiateTraits(this);
		}

		private void LoadInitializationFile()
		{
			if (_initializationFile == null) return;

			Debug.Log("Loading initialization file.");

			var input = new StringReader(_initializationFile.text);

			var yaml = new YamlStream();
			yaml.Load(input);

			// The root of initialization file is a mapping of TDRSNode IDs
			// to settings for initializing that Nodes traits, stats, and
			// outgoing relationships
			var rootMapping = (YamlMappingNode)yaml.Documents[0].RootNode;

			foreach (var (key, nodeSettings) in rootMapping.Children)
			{
				// Get the node ID
				var nodeID = key.GetValue();
				var node = GetNode(nodeID);

				// Configure initial base stats
				YamlNode statsNode = nodeSettings.TryGetChild("base_stats");
				if (statsNode != null)
				{
					foreach (var pair in statsNode.GetChildren())
					{
						var stat = pair.Key.GetValue();
						var baseValue = float.Parse(pair.Value.GetValue());

						node.Stats.GetStat(stat).BaseValue = baseValue;
					}
				}

				// Configure initial traits
				var traitsNode = nodeSettings.TryGetChild("traits");
				if (traitsNode != null)
				{
					var traits = ((YamlSequenceNode)traitsNode).Select(x => x.GetValue()).ToList();
					foreach (var traitID in traits)
					{
						AddTraitToNode(nodeID, traitID);
					}
				}

				// Configure initial relationships
				var relationshipsNode = nodeSettings.TryGetChild("relationships");
				if (relationshipsNode != null)
				{
					foreach (var pair in relationshipsNode.GetChildren())
					{
						var targetID = pair.Key.GetValue();
						var relationshipSettings = (YamlMappingNode)pair.Value;

						// Configure initial relationship base stats
						var relationship = GetRelationship(nodeID, targetID);

						YamlNode relationshipStatsNode = relationshipSettings.TryGetChild("base_stats");
						if (relationshipStatsNode != null)
						{
							foreach (var relStatPair in relationshipStatsNode.GetChildren())
							{
								var stat = relStatPair.Key.GetValue();
								var baseValue = float.Parse(relStatPair.Value.GetValue());

								relationship.Stats.GetStat(stat).BaseValue = baseValue;
							}
						}

						// Configure initial relationship traits
						var relTraitsNode = relationshipSettings.TryGetChild("traits");
						if (relTraitsNode != null)
						{
							var relationshipTraits = ((YamlSequenceNode)relTraitsNode)
							.Select(x => x.GetValue()).ToList();
							foreach (var traitID in relationshipTraits)
							{
								AddTraitToRelationship(nodeID, targetID, traitID);
							}
						}
					}
				}
			}



		}

		#endregion

		#region Methods
		/// <summary>
		/// Retrieves the social entity with the given ID or creates one
		/// if one is not found.
		/// </summary>
		/// <param name="entityID"></param>
		/// <returns></returns>
		public TDRSNode GetNode(string entityID)
		{
			if (_nodes.ContainsKey(entityID))
			{
				return _nodes[entityID];
			}

			var node = new TDRSNode(this, entityID);

			foreach (var entry in _entityStats)
			{
				node.Stats.AddStat(entry.statName, new StatSystem.Stat(
					entry.baseValue, entry.minValue, entry.maxValue, entry.isDiscrete
				));
			}

			_nodes[entityID] = node;

			return node;
		}

		/// <summary>
		/// Creates gets the relationship from the owner to target and creates a
		/// new relationship if one does not exist.
		///
		/// <para>
		/// This adds the necessary stats active social rules when creating new relationships
		/// </para>
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="targetID"></param>
		/// <returns></returns>
		public TDRSRelationship GetRelationship(string ownerID, string targetID)
		{
			var owner = GetNode(ownerID);
			var target = GetNode(targetID);

			if (owner.OutgoingRelationships.ContainsKey(target))
			{
				return owner.OutgoingRelationships[target];
			}

			var relationship = new TDRSRelationship(
				this, $"{ownerID}->{targetID}", owner, target
			);

			owner.OutgoingRelationships[target] = relationship;
			target.IncomingRelationships[owner] = relationship;

			// Configure stats
			foreach (var entry in _relationshipStats)
			{
				relationship.Stats.AddStat(entry.statName, new StatSystem.Stat(
					entry.baseValue, entry.minValue, entry.maxValue, entry.isDiscrete
				));
			}

			// Apply outgoing social rules from the owner
			foreach (var rule in owner.SocialRules.Rules)
			{
				if (rule.IsOutgoing && rule.CheckPreconditions(relationship))
				{
					rule.OnAdd(relationship);
					relationship.SocialRules.AddSocialRule(rule);
				}
			}

			// Apply incoming social rules from the target
			foreach (var rule in target.SocialRules.Rules)
			{
				if (!rule.IsOutgoing && rule.CheckPreconditions(relationship))
				{
					rule.OnAdd(relationship);
					relationship.SocialRules.AddSocialRule(rule);
				}
			}

			return relationship;
		}

		/// <summary>
		/// Add a trait to an entity
		/// </summary>
		/// <param name="entityID"></param>
		/// <param name="traitID"></param>
		public void AddTraitToNode(string entityID, string traitID)
		{
			var node = GetNode(entityID);
			var trait = TraitLibrary.GetTrait(traitID);
			node.Traits.AddTrait(trait);
			trait.OnAdd(node);
		}

		/// <summary>
		/// Remove a trait from a node
		/// </summary>
		/// <param name="entityID"></param>
		/// <param name="traitID"></param>
		public void RemoveTraitFromNode(string entityID, string traitID)
		{
			var node = GetNode(entityID);
			var trait = TraitLibrary.GetTrait(traitID);
			node.Traits.RemoveTrait(trait);
			trait.OnRemove(node);
		}

		/// <summary>
		/// Remove a social rule from a node
		/// </summary>
		/// <param name="entityID"></param>
		/// <param name="socialRule"></param>
		public void AddSocialRuleToNode(string entityID, SocialRule socialRule)
		{
			var node = GetNode(entityID);
			node.SocialRules.AddSocialRule(socialRule);
		}

		/// <summary>
		/// Remove a social rule from a node
		/// </summary>
		/// <param name="entityID"></param>
		/// <param name="socialRule"></param>
		public void RemoveSocialRuleFromNode(string entityID, SocialRule socialRule)
		{
			var node = GetNode(entityID);
			node.SocialRules.RemoveSocialRule(socialRule);
		}

		/// <summary>
		/// Remove all social rules on a node from a given source
		/// </summary>
		/// <param name="entityID"></param>
		/// <param name="socialRule"></param>
		public void RemoveAllSocialRulesFromSource(string entityID, object source)
		{
			var node = GetNode(entityID);
			var socialRules = node.SocialRules.Rules.ToList();
			for (int i = socialRules.Count(); i >= 0; i--)
			{
				var rule = socialRules[i];
				if (rule.Source == source)
				{
					RemoveSocialRuleFromNode(entityID, rule);
				}
			}
		}

		/// <summary>
		/// Remove a trait from a relationship
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="targetID"></param>
		/// <param name="traitID"></param>
		public void AddTraitToRelationship(string ownerID, string targetID, string traitID)
		{
			var relationship = GetRelationship(ownerID, targetID);
			var trait = TraitLibrary.GetTrait(traitID);
			relationship.Traits.AddTrait(trait);
			trait.OnAdd(relationship);
		}


		/// <summary>
		/// Adds a trait to the relationship between two characters
		/// </summary>
		/// <param name="ownerID"></param>
		/// <param name="targetID"></param>
		/// <param name="traitID"></param>
		public void RemoveTraitFromRelationship(string ownerID, string targetID, string traitID)
		{
			var relationship = GetRelationship(ownerID, targetID);
			var trait = TraitLibrary.GetTrait(traitID);
			relationship.Traits.RemoveTrait(trait);
			trait.OnRemove(relationship);
		}
		#endregion
	}
}
