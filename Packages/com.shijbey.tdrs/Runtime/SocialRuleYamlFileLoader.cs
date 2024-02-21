using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// Load Trait Definitions from YAML files.
	/// </summary>
	public class SocialRuleYamlFileLoader : MonoBehaviour
	{
		/// <summary>
		/// YAML files containing traits to load.
		/// </summary>
		[SerializeField]
		private TextAsset[] m_ruleYamlFiles;

		private void OnEnable()
		{
			SocialEngineController.OnLoadSocialRules += LoadSocialRules;
		}

		private void OnDisable()
		{
			SocialEngineController.OnLoadSocialRules -= LoadSocialRules;
		}

		/// <summary>
		/// Load social rules from the text assets.
		/// </summary>
		public void LoadSocialRules(SocialEngine state)
		{
			var loader = new SocialRuleYamlLoader();

			foreach (var textAsset in m_ruleYamlFiles)
			{
				if (textAsset == null) continue;

				var definitions = loader.LoadSocialRules(textAsset.text);

				foreach (var entry in definitions)
				{
					state.AddSocialRule(entry);
				}
			}
		}
	}
}
