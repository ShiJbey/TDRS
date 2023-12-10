using System.Collections.Generic;
using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// A vertex/node within the social graph. This might represent a character, faction, concept,
	/// or any entity that characters might have a relationship toward.
	/// </summary>
	public class TDRSNode : SocialEntity
	{
		#region Properties

		/// <summary>
		/// A GameObject associated with this entity
		/// </summary>
		public GameObject GameObject { get; set; }

		/// <summary>
		/// Relationships directed toward this entity
		/// </summary>
		public Dictionary<TDRSNode, TDRSRelationship> IncomingRelationships { get; }

		/// <summary>
		/// Relationships from this entity directed toward other entities
		/// </summary>
		public Dictionary<TDRSNode, TDRSRelationship> OutgoingRelationships { get; }

		#endregion

		#region Constructors

		public TDRSNode(
			TDRSManager manager,
			string entityID
		) : base(manager, entityID)
		{
			GameObject = null;
			IncomingRelationships = new Dictionary<TDRSNode, TDRSRelationship>();
			OutgoingRelationships = new Dictionary<TDRSNode, TDRSRelationship>();
		}

		public TDRSNode(
			TDRSManager manager,
			string entityID,
			GameObject gameObject
		) : base(manager, entityID)
		{
			GameObject = gameObject;
			Traits = new TDRSNodeTraits(this);
			SocialRules = new TDRSNodeSocialRules(this);
			IncomingRelationships = new Dictionary<TDRSNode, TDRSRelationship>();
			OutgoingRelationships = new Dictionary<TDRSNode, TDRSRelationship>();
		}

		#endregion
	}
}
