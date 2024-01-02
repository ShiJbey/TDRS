using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// An edge that connects two nodes within the social graph. This represents a social
	/// relationship between two entities.
	/// </summary>
	public class TDRSRelationship : SocialEntity
	{
		#region Properties

		public GameObject GameObject { get; }

		/// <summary>
		/// The entity that owns the relationship
		/// </summary>
		public TDRSNode Owner { get; }

		/// <summary>
		/// The entity the relationship is directed toward
		/// </summary>
		public TDRSNode Target { get; }

		#endregion

		#region Constructors

		public TDRSRelationship(
			SocialEngine engine,
			string entityID,
			TDRSNode owner,
			TDRSNode target,
			GameObject gameObject
		) : base(engine, entityID)
		{
			Owner = owner;
			Target = target;
			GameObject = gameObject;

			Traits.OnTraitAdded += (traits, traitID) =>
			{
				Engine.DB.Insert($"{Owner.UID}.relationship.{Target.UID}.trait.{traitID}");
			};

			Traits.OnTraitRemoved += (traits, traitID) =>
			{
				Engine.DB.Delete($"{Owner.UID}.relationship.{Target.UID}.trait.{traitID}");
			};

			Stats.OnValueChanged += (stats, pair) =>
			{
				string statName = pair.Item1;
				float value = pair.Item2;

				Engine.DB.Insert($"{Owner.UID}.relationship.{Target.UID}.stat.{statName}!{value}");
			};
		}

		#endregion


	}
}
