using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// Load Trait Definitions from YAML files.
	/// </summary>
	public class TraitYamlFileLoader : MonoBehaviour
	{
		/// <summary>
		/// A list of file paths relative to the StreamingAssets directory
		/// </summary>
		[SerializeField]
		private TextAsset[] m_traitYamlFiles;

		private void OnEnable()
		{
			SocialEngineController.OnLoadTraits += LoadTraitDefinitions;
		}

		private void OnDisable()
		{
			SocialEngineController.OnLoadTraits -= LoadTraitDefinitions;
		}

		/// <summary>
		/// Load trait definitions from definition filed provided in the inspector
		/// </summary>
		public void LoadTraitDefinitions(SocialEngine state)
		{
			var loader = new TraitYamlLoader();

			foreach (var textAsset in m_traitYamlFiles)
			{
				if (textAsset == null) continue;

				var definitions = loader.LoadTraits(textAsset.text);

				foreach (var traitDefinition in definitions)
				{
					state.TraitLibrary.Traits[traitDefinition.TraitID] = traitDefinition;
				}
			}
		}
	}
}
