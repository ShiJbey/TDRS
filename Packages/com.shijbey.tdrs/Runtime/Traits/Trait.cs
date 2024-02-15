using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// An instance of a trait definition that has been applied to an agent or relationship.
	/// </summary>
	public class Trait : IEffectSource
	{
		#region Fields

		protected List<SocialRule> m_socialRules;

		#endregion

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
		public string Description { get; set; }

		/// <summary>
		/// Configuration data for effects associated for this trait.
		/// </summary>
		public List<string> Effects { get; set; }

		/// <summary>
		/// IDs of traits that this trait cannot be added with.
		/// </summary>
		public HashSet<string> ConflictingTraits { get; set; }

		/// <summary>
		/// Social rules associated with this trait applied to relationships.
		/// </summary>
		public List<SocialRule> SocialRules
		{
			get
			{
				return m_socialRules;
			}

			set
			{
				m_socialRules = value;
				foreach (var socialRule in m_socialRules)
				{
					socialRule.Source = this;
				}
			}
		}

		// From IEffectSource interface
		public string EffectSourceID => $"trait/{TraitID}";

		#endregion

		#region Constructors

		public Trait(
			string traitID,
			TraitType traitType,
			string displayName
		)
		{
			TraitID = traitID;
			TraitType = traitType;
			DisplayName = displayName;
			Description = "";
			Effects = new List<string>();
			SocialRules = new List<SocialRule>();
			ConflictingTraits = new HashSet<string>();
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
