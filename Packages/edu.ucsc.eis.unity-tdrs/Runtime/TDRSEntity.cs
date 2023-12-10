using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace TDRS
{
	/// <summary>
	/// A user-facing Unity component for associating a GameObject with as node within
	/// the TDRS Manager's social graph.
	/// </summary>
	public class TDRSEntity : MonoBehaviour, ISerializationCallbackReceiver
	{
		#region Attributes

		/// <summary>
		/// The ID of the entity within the TDRS Manager
		/// </summary>
		[SerializeField]
		public string entityID = "";

		/// <summary>
		/// A reference to the corresponding node within the TDRS Manager
		/// </summary>
		protected TDRSNode _node;

		/// <summary>
		/// Serialized list of trait IDs from the the TDRSNode
		/// </summary>
		[Space(16)]
		[SerializeField]
		private List<string> traits;

		/// <summary>
		/// Serialized list of stat data from the TDRSNode
		/// </summary>
		[Space(12)]
		[SerializeField]
		private SerializedStatData stats;

		/// <summary>
		/// Serialized list of relationship data from the TDRSNode
		/// </summary>
		[Space(12)]
		[SerializeField]
		private List<SerializedTDRSRelationship> relationships;

		/// <summary>
		/// (Experimental) Event triggered when a trait is added to the node associated with
		/// this MonoBehaviour
		/// </summary>
		[Space(16)]
		[Header("Event Listeners")]
		[Space(12)]
		public TraitAddedEvent OnTraitAdded;

		/// <summary>
		/// (Experimental) Event triggered when a trait is removed from the node associated with
		/// this MonoBehaviour
		/// </summary>
		public TraitRemovedEvent OnTraitRemoved;

		/// <summary>
		/// (Experimental) Event triggered when a stat is changed on the node associated with
		/// this MonoBehaviour
		/// </summary>
		private StatChangeEvent OnStatChange;

		/// <summary>
		/// (Experimental) Event triggered when a new outgoing relationship is added to from the
		/// node associated with this MonoBehaviour
		/// </summary>
		private NewRelationshipEvent OnNewRelationship;

		#endregion

		#region Unity Methods

		void Start()
		{
			// Get a reference to this GameObject's node within the social graph
			// and assign this GameObject to be the node's GameObject
			_node = TDRSManager.Instance.GetNode(entityID);
			_node.GameObject = gameObject;

			Debug.Log($"{entityID} has retrieved their TDRS Node.");
		}

		public void OnBeforeSerialize()
		{
			traits.Clear();

			relationships.Clear();

			if (_node == null) return;

			traits = _node.Traits.GetAllTraits().Select(t => t.TraitID).ToList();

			stats = new SerializedStatData()
			{
				stats = _node.Stats.GetStats().Select(pair => new SerializedStat()
				{
					statName = pair.Key,
					baseValue = pair.Value.BaseValue,
					value = pair.Value.Value
				}).ToList(),
				modifiers = _node.Stats.Modifiers.Select(m => m.Description).ToList(),
			};

			relationships = _node.OutgoingRelationships.Values.Select(
				rel => new SerializedTDRSRelationship()
				{
					target = rel.Target.EntityID,
					traits = rel.Traits.GetAllTraits().Select(t => t.TraitID).ToList(),
					stats = new SerializedStatData()
					{
						stats = rel.Stats.GetStats().Select(pair => new SerializedStat()
						{
							statName = pair.Key,
							baseValue = pair.Value.BaseValue,
							value = pair.Value.Value
						}).ToList(),
						modifiers = rel.Stats.Modifiers.Select(m => m.Description).ToList()
					}
				}
			).ToList();
		}

		public void OnAfterDeserialize()
		{
			// if (_node == null) return;

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
			// foreach (var entry in stats)
			// {
			// 	_node.Stats[entry.statName].BaseValue = entry.baseValue;
			// }
		}

		#endregion
	}

	#region Custom Event Classes

	/// <summary>
	/// Event dispatched when a trait is added to a social entity
	/// </summary>
	[System.Serializable]
	public class TraitAddedEvent : UnityEvent<Trait> { }

	/// <summary>
	/// Event dispatched when a trait is removed from a social entity
	/// </summary>
	[System.Serializable]
	public class TraitRemovedEvent : UnityEvent<Trait> { }

	/// <summary>
	/// Event dispatched when a social entity has a stat that is changed
	/// </summary>
	[System.Serializable]
	public class StatChangeEvent : UnityEvent<string, float> { }

	/// <summary>
	/// Event dispatched when a social entity gains a new outgoing relationship
	/// </summary>
	[System.Serializable]
	public class NewRelationshipEvent : UnityEvent<TDRSRelationship> { }

	#endregion

	#region Helper Classes

	/// <summary>
	/// Serialized information about stat values
	/// </summary>
	[System.Serializable]
	public class SerializedStat
	{
		public string statName;
		public float baseValue;
		public float value;
	}

	/// <summary>
	/// Serialized information about stat values
	/// </summary>
	[System.Serializable]
	public class SerializedStatData
	{
		public List<SerializedStat> stats;
		public List<string> modifiers;
	}

	/// <summary>
	/// Serialized information about a relationship instance
	/// </summary>
	[System.Serializable]
	public class SerializedTDRSRelationship
	{
		public string target;
		public List<string> traits;
		public SerializedStatData stats;
	}

	#endregion
}
