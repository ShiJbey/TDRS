using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// A record of a trait that has been applied to an agent or relationship.
	/// </summary>
	public class TraitInstance
	{
		#region Fields

		/// <summary>
		/// The trait that this object is an instance of.
		/// </summary>
		protected Trait m_trait;

		#endregion

		#region Properties

		/// <summary>
		/// The target of this effect.
		/// </summary>
		public ISocialEntity Target { get; }

		/// <summary>
		/// The unique ID of the trait.
		/// </summary>
		public string TraitID => m_trait.TraitID;

		/// <summary>
		/// The name of the trait as displayed in GUIs.
		/// </summary>
		public string DisplayName => m_trait.DisplayName;

		/// <summary>
		/// The type of object this trait is applied to.
		/// </summary>
		public TraitType TraitType => m_trait.TraitType;

		/// <summary>
		/// A short textual description of the trait.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// IDs of traits that this trait cannot be added with.
		/// </summary>
		public HashSet<string> ConflictingTraits => m_trait.ConflictingTraits;

		/// <summary>
		/// Set to true when this trait uses a duration.
		/// </summary>
		public bool HasDuration { get; }

		/// <summary>
		/// The amount of time remaining for this trait.
		/// </summary>
		public int Duration { get; protected set; }

		#endregion

		#region Constructors

		public TraitInstance(
			ISocialEntity target,
			Trait trait,
			string description,
			int duration = -1
		)
		{
			m_trait = trait;
			Target = target;
			Description = description;
			HasDuration = duration > 0;
			Duration = duration;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Update the trait instance for the current engine time tick.
		/// </summary>
		public void Tick()
		{
			if (HasDuration)
			{
				Duration = -1;
			}
		}

		/// <summary>
		/// Apply this rule's modifiers to a relationship.
		/// </summary>
		/// <param name="entity"></param>
		public void ApplyModifiers()
		{
			foreach (var modifierData in m_trait.Modifiers)
			{
				Target.Stats.GetStat(modifierData.StatName).AddModifier(
					new StatModifier(
						modifierData.Value,
						modifierData.ModifierType,
						this
					)
				);
			}
		}

		/// <summary>
		/// Remove this rule's modifiers from a relationship.
		/// </summary>
		/// <param name="entity"></param>
		public void RemoveModifiers()
		{
			foreach (var modifierData in m_trait.Modifiers)
			{
				Target.Stats.GetStat(modifierData.StatName).RemoveModifiersFromSource(this);
			}
		}

		#endregion
	}
}
