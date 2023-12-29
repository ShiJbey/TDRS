using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
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
		#region Attributes

		/// <summary>
		/// A list of TextAssets assigned within the Unity inspector
		/// </summary>
		[SerializeField]
		protected List<TextAsset> _traitDefinitions = new List<TextAsset>();

		/// <summary>
		/// A reference to a YAML initialization file for the social graph.
		/// </summary>
		[SerializeField]
		protected TextAsset _initializationFile;

		public LoadFactoriesEvent OnLoadFactories;

		[SerializeField]
		private List<NodeSchemaScriptableObj> _nodeSchemas;

		[SerializeField]
		private List<RelationshipSchemaScriptableObj> _relationshipSchemas;

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
			}
		}

		void Start()
		{
			LoadSchemas();
			LoadFactories();
			LoadTraits();
			LoadInitializationFile();
		}
		#endregion

		#region Content Loading Methods

		private void LoadSchemas()
		{
			foreach (NodeSchemaScriptableObj schema in _nodeSchemas)
			{
				SocialEngine.AddNodeSchema(schema.GetSchema());
			}

			foreach (RelationshipSchemaScriptableObj schema in _relationshipSchemas)
			{
				SocialEngine.AddRelationshipSchema(schema.GetSchema());
			}
		}

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
			var rootMapping = yaml.Documents[0].RootNode;

			foreach (var (key, nodeSettings) in rootMapping.GetChildren())
			{
				// Get the node ID
				var nodeID = key.GetValue();
				var node = SocialEngine.CreateNode("character", nodeID);

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
				// var traitsNode = nodeSettings.TryGetChild("traits");
				// if (traitsNode != null)
				// {
				// 	var traits = ((YamlSequenceNode)traitsNode).Select(x => x.GetValue()).ToList();
				// 	foreach (var traitID in traits)
				// 	{
				// 		// AddTraitToNode(nodeID, traitID);
				// 	}
				// }

				// Configure initial relationships
				// var relationshipsNode = nodeSettings.TryGetChild("relationships");
				// if (relationshipsNode != null)
				// {
				// 	foreach (var pair in relationshipsNode.GetChildren())
				// 	{
				// 		var targetID = pair.Key.GetValue();
				// 		var relationshipSettings = (YamlMappingNode)pair.Value;

				// 		// Configure initial relationship base stats
				// 		var relationship = SocialEngine.GetRelationship(nodeID, targetID);

				// 		YamlNode relationshipStatsNode = relationshipSettings.TryGetChild("base_stats");
				// 		if (relationshipStatsNode != null)
				// 		{
				// 			foreach (var relStatPair in relationshipStatsNode.GetChildren())
				// 			{
				// 				var stat = relStatPair.Key.GetValue();
				// 				var baseValue = float.Parse(relStatPair.Value.GetValue());

				// 				relationship.Stats.GetStat(stat).BaseValue = baseValue;
				// 			}
				// 		}

				// 		// Configure initial relationship traits
				// 		var relTraitsNode = relationshipSettings.TryGetChild("traits");
				// 		if (relTraitsNode != null)
				// 		{
				// 			var relationshipTraits = ((YamlSequenceNode)relTraitsNode)
				// 			.Select(x => x.GetValue()).ToList();
				// 			foreach (var traitID in relationshipTraits)
				// 			{
				// 				// AddTraitToRelationship(nodeID, targetID, traitID);
				// 			}
				// 		}
				// 	}
				// }
			}



		}

		#endregion
	}

	#region Custom Event Classes

	[Serializable]
	/// <summary>
	/// Event dispatched when the TDRSmanager is loading factory instances during start.
	/// </summary>
	public class LoadFactoriesEvent : UnityEvent<SocialEngine> { }

	#endregion
}
