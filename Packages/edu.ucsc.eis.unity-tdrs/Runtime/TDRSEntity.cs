using UnityEngine;
using UnityEngine.Events;

namespace TDRS
{
	/// <summary>
	/// A user-facing Unity component for associating a GameObject with as node within
	/// the TDRS Manager's social graph.
	/// </summary>
	[DefaultExecutionOrder(1)]
	public class TDRSEntity : MonoBehaviour
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
				_node = TDRSManager.Instance.SocialEngine.GetNode(entityID);

				Debug.Log($"{entityID} has retrieved their TDRS Node.");
			}
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
}
