using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// Manages the traits attached to a social entity or relationship.
	/// </summary>
	public class Traits
	{
		#region Attributes

		/// <summary>
		/// Traits currently applied to the entity
		/// </summary>
		protected Dictionary<string, Trait> _traits;

		/// <summary>
		/// Collection of TraitID's that conflict with the current traits
		/// </summary>
		protected HashSet<string> _conflictingTraits;

		#endregion

		#region Constructors

		public Traits()
		{
			_traits = new Dictionary<string, Trait>();
			_conflictingTraits = new HashSet<string>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add a trait to the entity
		/// </remarks>
		/// <param name="trait"></param>
		/// <returns></returns>
		public bool AddTrait(Trait trait)
		{
			if (_traits.ContainsKey(trait.TraitID))
			{
				return false;
			}

			if (HasConflictingTrait(trait))
			{
				return false;
			}

			_traits[trait.TraitID] = trait;

			_conflictingTraits.UnionWith(trait.ConflictingTraits);

			return true;
		}

		/// <summary>
		/// Remove a trait
		/// </summary>
		/// <param name="trait"></param>
		/// <returns></returns>
		public bool RemoveTrait(Trait trait)
		{
			if (!_traits.ContainsKey(trait.TraitID))
			{
				return false;
			}

			_traits.Remove(trait.TraitID);

			_conflictingTraits.Clear();
			foreach (var (_, remainingTrait) in _traits)
			{
				_conflictingTraits.UnionWith(remainingTrait.ConflictingTraits);
			}

			return true;
		}

		/// <summary>
		/// Check if a trait is already present
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public bool HasTrait(string traitID)
		{
			return _traits.ContainsKey(traitID);
		}


		/// <summary>
		/// Check if a trait is already present
		/// </summary>
		/// <param name="trait"></param>
		/// <returns></returns>
		public bool HasTrait(Trait trait)
		{
			return _traits.ContainsKey(trait.TraitID);
		}

		/// <summary>
		/// Check if the there is already a conflicting trait present
		/// </summary>
		/// <param name="trait"></param>
		/// <returns></returns>
		public bool HasConflictingTrait(Trait trait)
		{
			return _conflictingTraits.Contains(trait.TraitID);
		}

		#endregion
	}
}
