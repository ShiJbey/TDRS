using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace TDRS
{
	/// <summary>
	/// An intermediate representation of trait information as it is loaded from YAML
	/// </summary>
	public class TraitDefinition
	{
		/// <summary>
		/// The unique ID of the trait
		/// </summary>
		public string TraitID { get; }

		/// <summary>
		/// The name of the trait as displayed in GUIs
		/// </summary>
		public string DisplayName { get; }

		/// <summary>
		/// A short textual description of the trait
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Configuration data for effects associated for this trait
		/// </summary>
		public YamlNode EffectData { get; }

		/// <summary>
		/// IDs of traits that this trait cannot be added with
		/// </summary>
		public HashSet<string> ConflictingTraits { get; }

		/// <summary>
		/// How long the trait lasts (-1 is indefinite)
		/// </summary>
		public int Duration { get; }

		public TraitDefinition(
			string traitID,
			string displayName,
			string description,
			YamlNode effectData,
			HashSet<string> conflictingTraits,
			int duration = -1
		)
		{
			TraitID = traitID;
			DisplayName = displayName;
			Description = description;
			EffectData = effectData;
			ConflictingTraits = conflictingTraits;
			Duration = duration;
		}
	}
}
