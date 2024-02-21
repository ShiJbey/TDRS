using System.Collections.Generic;

namespace TDRS.Serialization
{
	public class SerializedTrait
	{
		#region Properties

		/// <summary>
		/// The unique ID of the trait.
		/// </summary>
		public string traitID { get; set; }

		/// <summary>
		/// The name of the trait as displayed in GUIs.
		/// </summary>
		public string displayName { get; set; }

		/// <summary>
		/// The type of object this trait is applied to.
		/// </summary>
		public string traitType { get; set; }

		/// <summary>
		/// A short textual description of the trait.
		/// </summary>
		public string description { get; set; }

		/// <summary>
		/// Stat modifiers to apply to the object this trait is attached to.
		/// </summary>
		public SerializedStatModifierData[] modifiers { get; set; }

		/// <summary>
		/// IDs of traits that this trait cannot be added with.
		/// </summary>
		public string[] conflictingTraits { get; set; }

		#endregion

		#region Constructors

		public SerializedTrait()
		{
			traitID = "";
			traitType = "Agent";
			displayName = "";
			description = "";
			modifiers = new SerializedStatModifierData[0];
			conflictingTraits = new string[0];
		}

		#endregion

		#region Public Methods

		public Trait ToRuntimeInstance()
		{
			var modifiers = new List<StatModifierData>();
			foreach (var serializedModifierData in this.modifiers)
			{
				modifiers.Add(
					new StatModifierData(
						serializedModifierData.statName,
						serializedModifierData.value,
						StatModifierType.Parse<StatModifierType>(
							serializedModifierData.modifierType, true
						)
					)
				);
			}

			TraitType traitType = TDRS.TraitType.Parse<TraitType>(this.traitType, true);

			return new Trait(
				traitID, traitType, displayName, description, modifiers, conflictingTraits);
		}

		#endregion
	}
}
