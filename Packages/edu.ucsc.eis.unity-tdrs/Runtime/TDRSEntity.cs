using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TDRS
{
	public class TDRSEntity : MonoBehaviour
	{
		[SerializeField]
		public string entityID = "";
		public List<string> traitsAtStart = new List<string>();
		protected TDRSNode _node;

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
	}

	[System.Serializable]
	public class TraitAddedEvent : UnityEvent<Trait> { }

	[System.Serializable]
	public class TraitRemovedEvent : UnityEvent<Trait> { }

	// [System.Serializable]
	// public class StatChangeEvent : UnityEvent<string, float> { }

	// [System.Serializable]
	// public class NewRelationshipEvent : UnityEvent<TDRSRelationship> { }
}
