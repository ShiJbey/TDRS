using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// An instance of a trait definition that has been applied to an agent or relationship.
	/// </summary>
	public class Trait
	{
		#region Properties

		/// <summary>
		/// The unique ID of the trait.
		/// </summary>
		public string TraitID { get; }

		/// <summary>
		/// The name of the trait as displayed in GUIs.
		/// </summary>
		public string DisplayName { get; }

		/// <summary>
		/// The type of object this trait is applied to.
		/// </summary>
		public TraitType TraitType { get; }

		/// <summary>
		/// A short textual description of the trait.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Stat modifiers to apply to the object this trait is attached to.
		/// </summary>
		public StatModifierData[] Modifiers { get; }

		/// <summary>
		/// IDs of traits that this trait cannot be added with.
		/// </summary>
		public HashSet<string> ConflictingTraits { get; }

		#endregion

		#region Constructors

		public Trait(
			string traitID,
			TraitType traitType,
			string displayName,
			string description,
			IEnumerable<StatModifierData> modifiers,
			IEnumerable<string> conflictingTraits
		)
		{
			TraitID = traitID;
			TraitType = traitType;
			DisplayName = displayName;
			Description = description;
			Modifiers = new List<StatModifierData>(modifiers).ToArray();
			ConflictingTraits = new HashSet<string>(conflictingTraits);
		}

		#endregion

		#region Public Methods

		public override string ToString()
		{
			return $"Trait({DisplayName})";
		}

		#endregion
	}
}
