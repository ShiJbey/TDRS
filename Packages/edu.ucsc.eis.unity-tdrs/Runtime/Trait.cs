using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// TraitTypes manage metadata associated with Traits.
	///
	/// TraitTypes create instances of traits and allow the
	/// OpinionSystem to save memory by only having a single
	/// copy of trait metadata.
	/// </summary>
	public class Trait
	{
		#region Fields

		/// <summary>
		/// The definition for this trait instance
		/// </summary>
		protected TraitDefinition m_definition;

		/// <summary>
		/// The hydrated description template
		/// </summary>
		protected string m_description;

		/// <summary>
		/// Remaining time for this traits to exist
		/// </summary>
		protected int m_duration;

		/// <summary>
		/// Effects to apply when the trait is added
		/// </summary>
		protected ISocialEventEffect[] m_effects;

		#endregion

		#region Properties

		/// <summary>
		/// A unique ID for this Trait.
		/// </summary>
		public string TraitID => m_definition.TraitID;

		/// <summary>
		/// An user-friendly name to use for GUIs.
		/// </summary>
		public string DisplayName => m_definition.DisplayName;

		/// <summary>
		/// A short description of the trait.
		/// </summary>
		public string Description => m_description;

		/// <summary>
		/// The amount of simulation ticks this trait lasts for
		/// </summary>
		public int Duration => m_duration;

		/// <summary>
		/// Effects to apply when the trait is added
		/// </summary>
		public IEnumerable<ISocialEventEffect> Effects => m_effects;

		/// <summary>
		/// IDs of traits that this trait cannot be added with
		/// </summary>
		public HashSet<string> ConflictingTraits => m_definition.ConflictingTraits;

		/// <summary>
		/// The social rules associated with this trait
		/// </summary>
		public SocialRuleDefinition[] SocialRuleDefinitions => m_definition.SocialRules;

		#endregion

		#region Constructors

		public Trait(
			TraitDefinition traitDefinition,
			ISocialEventEffect[] effects,
			string description
		)
		{
			m_definition = traitDefinition;
			m_effects = effects;
			m_duration = traitDefinition.Duration;
			m_description = description;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Override the current duration for the trait
		/// </summary>
		/// <param name="duration"></param>
		public void SetDuration(int duration)
		{
			m_duration = duration;
		}

		/// <summary>
		/// Decrease the time left for this trait by one time tick
		/// </summary>
		public void DecrementDuration()
		{
			m_duration -= 1;
		}

		public override string ToString()
		{
			return DisplayName;
		}

		#endregion
	}
}
