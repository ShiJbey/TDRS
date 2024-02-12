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
		/// The unique ID of the trait
		/// </summary>
		public string TraitID { get; }

		/// <summary>
		/// The name of the trait as displayed in GUIs
		/// </summary>
		public string DisplayName { get; }

		/// <summary>
		/// The type of object this trait is applied to
		/// </summary>
		public TraitType TraitType { get; }

		/// <summary>
		/// A short textual description of the trait
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Configuration data for effects associated for this trait
		/// </summary>
		public List<string> Effects { get; }

		/// <summary>
		/// IDs of traits that this trait cannot be added with
		/// </summary>
		public HashSet<string> ConflictingTraits { get; }

		/// <summary>
		/// Social rules associated with this trait
		/// </summary>
		public List<SocialRule> SocialRules { get; }

		#endregion

		#region Constructors

		public Trait(
			string traitID,
			TraitType traitType,
			string displayName,
			string description
		)
		{
			TraitID = traitID;
			TraitType = traitType;
			DisplayName = displayName;
			Description = description;
			Effects = new List<string>();
			SocialRules = new List<SocialRule>();
			ConflictingTraits = new HashSet<string>();
		}

		public Trait(
			string traitID,
			TraitType traitType,
			string displayName,
			string description,
			IEnumerable<string> effects,
			IEnumerable<SocialRule> socialRules,
			IEnumerable<string> conflictingTraits
		)
		{
			TraitID = traitID;
			TraitType = traitType;
			DisplayName = displayName;
			Description = description;
			Effects = new List<string>(effects);
			SocialRules = new List<SocialRule>(socialRules);
			ConflictingTraits = new HashSet<string>(conflictingTraits);

			foreach (var socialRule in SocialRules)
			{
				socialRule.Source = this;
			}
		}

		#endregion

		#region Public Methods

		public override string ToString()
		{
			return DisplayName;
		}

		#endregion
	}
}
