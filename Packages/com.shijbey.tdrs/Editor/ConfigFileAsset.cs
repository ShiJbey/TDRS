using UnityEditor;

namespace TDRS
{
	public class ConfigFileAsset
	{
		[MenuItem("Assets/Create/TDRS/Traits YAML", false, 1)]
		private static void CreateNewTraitDefinitionsYaml()
		{
			ProjectWindowUtil.CreateAssetWithContent(
				"traits.yaml",
				string.Empty);
		}

		[MenuItem("Assets/Create/TDRS/Social Events YAML", false, 1)]
		private static void CreateNewSocialEventsYaml()
		{
			ProjectWindowUtil.CreateAssetWithContent(
				"social_events.yaml",
				string.Empty);
		}

		[MenuItem("Assets/Create/TDRS/Social Rules YAML", false, 1)]
		private static void CreateNewSocialRulesYaml()
		{
			ProjectWindowUtil.CreateAssetWithContent(
				"social_rules.yaml",
				string.Empty);
		}
	}
}
