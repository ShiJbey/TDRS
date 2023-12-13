using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
		protected TDRSNode _node = null;

		/// <summary>
		/// Serialized list of trait IDs from the the TDRSNode
		/// </summary>
		[SerializeField]
		private List<string> traits = new List<string>();

		/// <summary>
		/// Serialized list of stat data from the TDRSNode
		/// </summary>
		[SerializeField]
		private SerializedStatData stats = new SerializedStatData();

		/// <summary>
		/// Serialized list of relationship data from the TDRSNode
		/// </summary>
		[SerializeField]
		private List<SerializedTDRSRelationship> relationships = new List<SerializedTDRSRelationship>();

		/// <summary>
		/// (Experimental) Event triggered when a trait is added to the node associated with
		/// this MonoBehaviour
		/// </summary>
		[Header("Event Listeners")]
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
			if (entityID != "")
			{
				// Get a reference to this GameObject's node within the social graph
				// and assign this GameObject to be the node's GameObject
				_node = TDRSManager.Instance.GetNode(entityID);
				_node.GameObject = gameObject;

				Debug.Log($"{entityID} has retrieved their TDRS Node.");
			}
		}

		public void OnBeforeSerialize()
		{
			traits.Clear();
			stats.stats.Clear();
			stats.modifiers.Clear();
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
		public string statName = "";
		public float baseValue = 0f;
		public float value = 0f;
	}

	/// <summary>
	/// Serialized information about stat values
	/// </summary>
	[System.Serializable]
	public class SerializedStatData
	{
		public List<SerializedStat> stats = new List<SerializedStat>();
		public List<string> modifiers = new List<string>();
	}

	/// <summary>
	/// Serialized information about a relationship instance
	/// </summary>
	[System.Serializable]
	public class SerializedTDRSRelationship
	{
		public string target = "";
		public List<string> traits = new List<string>();
		public SerializedStatData stats = new SerializedStatData();
	}

	#endregion
}
