using System;
using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// Manages the traits attached to a social entity or relationship.
	/// </summary>
	public class TraitManager
	{
		#region Fields

		/// <summary>
		/// Traits currently applied to the entity
		/// </summary>
		protected Dictionary<string, Trait> m_traits;

		#endregion

		#region Events

		/// <summary>
		/// Event published when a trait is added to the collection.
		/// </summary>
		public event EventHandler<OnTraitAddedArgs> OnTraitAdded;
		public class OnTraitAddedArgs : EventArgs
		{
			public Trait Trait { get; }

			public OnTraitAddedArgs(Trait trait)
			{
				Trait = trait;
			}
		}

		/// <summary>
		/// Event published when a trait is removed from the collection.
		/// </summary>
		public event EventHandler<OnTraitRemovedArgs> OnTraitRemoved;
		public class OnTraitRemovedArgs : EventArgs
		{
			public Trait Trait { get; }

			public OnTraitRemovedArgs(Trait trait)
			{
				Trait = trait;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// All traits within the collection.
		/// </summary>
		public IEnumerable<Trait> Traits => m_traits.Values;

		#endregion

		#region Constructors

		public TraitManager()
		{
			m_traits = new Dictionary<string, Trait>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a trait to the entity
		/// </remarks>
		/// <param name="trait"></param>
		/// <returns></returns>
		public bool AddTrait(Trait trait)
		{
			if (m_traits.ContainsKey(trait.TraitID)) return false;

			if (HasConflictingTrait(trait)) return false;

			m_traits[trait.TraitID] = trait;

			OnTraitAdded?.Invoke(this, new OnTraitAddedArgs(trait));

			return true;
		}

		/// <summary>
		/// Remove a trait
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public bool RemoveTrait(string traitID)
		{
			if (!m_traits.ContainsKey(traitID)) return false;

			var trait = GetTrait(traitID);

			m_traits.Remove(traitID);

			OnTraitRemoved?.Invoke(this, new OnTraitRemovedArgs(trait));

			return true;
		}

		/// <summary>
		/// Remove a trait
		/// </summary>
		/// <param name="trait"></param>
		/// <returns></returns>
		public bool RemoveTrait(Trait trait)
		{
			return RemoveTrait(trait.TraitID);
		}

		/// <summary>
		/// Check if a trait is already present
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public bool HasTrait(string traitID)
		{
			return m_traits.ContainsKey(traitID);
		}

		/// <summary>
		/// Check if a trait is already present
		/// </summary>
		/// <param name="trait"></param>
		/// <returns></returns>
		public bool HasTrait(Trait trait)
		{
			return m_traits.ContainsKey(trait.TraitID);
		}

		/// <summary>
		/// Check if the there is already a conflicting trait present
		/// </summary>
		/// <param name="trait"></param>
		/// <returns></returns>
		public bool HasConflictingTrait(Trait trait)
		{
			foreach (var (_, otherTrait) in m_traits)
			{
				if (trait.ConflictingTraits.Contains(otherTrait.TraitID))
				{
					return true;
				}

				if (otherTrait.ConflictingTraits.Contains(trait.TraitID))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Get a trait instance from the collection
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public Trait GetTrait(string traitID)
		{
			return m_traits[traitID];
		}

		#endregion
	}
}
