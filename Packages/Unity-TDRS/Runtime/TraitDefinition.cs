using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace TDRS
{
	public class TraitDefinition
	{
		public string TraitID { get; private set; }
		public string DisplayName { get; private set; }
		public string Description { get; private set; }
		public YamlNode EffectData { get; private set; }
		public HashSet<string> ConflictingTraits { get; private set; }

		public TraitDefinition(
			string traitID,
			string displayName,
			string description,
			YamlNode effectData,
			HashSet<string> conflictingTraits
		)
		{
			TraitID = traitID;
			DisplayName = displayName;
			Description = description;
			EffectData = effectData;
			ConflictingTraits = conflictingTraits;
		}
	}
}
