using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace TDRS
{
	public class TDRSEntity : MonoBehaviour, ISerializationCallbackReceiver
	{
		[SerializeField]
		public string entityID = "";
		public List<string> traitsAtStart = new List<string>();
		protected TDRSNode _node;

		[Space(16)]
		[SerializeField]
		private List<string> traits;

		[Space(12)]
		[SerializeField]
		private List<SerializedStatData> stats;

		[Space(12)]
		[SerializeField]
		private List<SerializedTDRSRelationship> relationships;

		[Space(16)]
		[Header("Event Listeners")]
		[Space(12)]
		public TraitAddedEvent OnTraitAdded;
		public TraitRemovedEvent OnTraitRemoved;
		// public StatChangeEvent OnStatChange;
		// public NewRelationshipEvent OnNewRelationship;

		// Start is called before the first frame update
		void Start()
		{
			// Get a reference to this GameObject's node within the social graph
			// and assign this GameObject to be the node's GameObject
			_node = TDRSManager.Instance.GetNode(entityID);
			_node.GameObject = gameObject;

			Debug.Log($"{entityID} has retrieved their TDRS Node.");

			// We need to synchronize the traits assigned to this script
			// and the traits that are present within the TDRSManager's
			// social graph

			// First, add the traits assigned within the inspector
			foreach (var traitID in traitsAtStart)
			{
				TDRSManager.Instance.AddTraitToNode(entityID, traitID);
			}

			// Next,
		}

		public void OnBeforeSerialize()
		{
			traits.Clear();
			stats.Clear();
			relationships.Clear();

			if (_node == null) return;

			traits = _node.Traits.GetAllTraits().Select(t => t.TraitID).ToList();

			stats = _node.Stats.Select(pair => new SerializedStatData()
			{
				statName = pair.Key,
				baseValue = pair.Value.BaseValue,
				value = pair.Value.Value
			}).ToList();

			relationships = _node.OutgoingRelationships.Values.Select(
				rel => new SerializedTDRSRelationship()
				{
					target = rel.Target.EntityID,
					traits = rel.Traits.GetAllTraits().Select(t => t.TraitID).ToList(),
					stats = rel.Stats.Select(pair => new SerializedStatData()
					{
						statName = pair.Key,
						baseValue = pair.Value.BaseValue,
						value = pair.Value.Value
					}).ToList(),
				}
			).ToList();
		}

		public void OnAfterDeserialize()
		{
			if (_node == null) return;

			// // Turn serialized trait data into runtime data
			// var serializedTraitSet = new HashSet<string>(traits);

			// var currentTraits = _node.Traits.GetAllTraits().Select(t => t.TraitID).ToList();

			// // First Remove traits that were removed in the inspector
			// foreach (var t in currentTraits)
			// {
			// 	if (!serializedTraitSet.Contains(t))
			// 	{
			// 		_node.Manager.RemoveTraitFromNode(_node.EntityID, t);
			// 	}
			// }

			// var currentTraitSet = new HashSet<string>(currentTraits);

			// // Add traits that were added in the inspector
			// foreach (var t in traits)
			// {
			// 	if (!currentTraitSet.Contains(t))
			// 	{
			// 		_node.Manager.AddTraitToNode(_node.EntityID, t);
			// 	}
			// }

			// Turn the serialized stat data back into runtime data
			foreach (var entry in stats)
			{
				_node.Stats[entry.statName].BaseValue = entry.baseValue;
			}
		}
	}

	[System.Serializable]
	public class TraitAddedEvent : UnityEvent<Trait> { }

	[System.Serializable]
	public class TraitRemovedEvent : UnityEvent<Trait> { }

	// [System.Serializable]
	// public class StatChangeEvent : UnityEvent<string, float> { }

	// [System.Serializable]
	// public class NewRelationshipEvent : UnityEvent<TDRSRelationship> { }

	#region Helper Classes
	[System.Serializable]
	public class SerializedStatData
	{
		public string statName;
		public float baseValue;
		public float value;
	}

	[System.Serializable]
	public class SerializedTDRSRelationship
	{
		public string target;
		public List<string> traits;
		public List<SerializedStatData> stats;
	}
	#endregion
}
