namespace TDRS
{
	/// <summary>
	/// Applies stat changes to a relationship based on a set of preconditions.
	/// </summary>
	public class SocialRule
	{
		#region Properties

		/// <summary>
		/// A template description to be filled when recording the rules effects on a relationship.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// RePraxis query clauses to run against the social engine's database.
		/// </summary>
		public string[] Preconditions { get; }

		/// <summary>
		/// Stat modifiers to apply if the preconditions pass.
		/// </summary>
		public StatModifierData[] Modifiers { get; }

		#endregion

		#region Constructors

		public SocialRule(
			string description,
			string[] preconditions,
			StatModifierData[] modifiers
		)
		{
			Description = description;
			Preconditions = preconditions;
			Modifiers = modifiers;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Apply this rule's modifiers to a relationship.
		/// </summary>
		/// <param name="relationship"></param>
		public void ApplyModifiers(Relationship relationship)
		{
			foreach (var modifierData in Modifiers)
			{
				relationship.Stats.GetStat(modifierData.StatName).AddModifier(
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
		/// <param name="relationship"></param>
		public void RemoveModifiers(Relationship relationship)
		{
			foreach (var modifierData in Modifiers)
			{
				relationship.Stats.GetStat(modifierData.StatName).RemoveModifiersFromSource(this);
			}
		}

		public override string ToString()
		{
			return $"SocialRule({Description})";
		}

		#endregion
	}
}
