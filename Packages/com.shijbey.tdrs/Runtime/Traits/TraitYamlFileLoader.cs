using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// Load Trait Definitions from YAML files.
	/// </summary>
	public class TraitYamlFileLoader : MonoBehaviour
	{
		/// <summary>
		/// YAML files containing traits to load.
		/// </summary>
		[SerializeField]
		private TextAsset[] m_traitYamlFiles;

		private void OnEnable()
		{
			SocialEngineController.OnLoadTraits += LoadTraits;
		}

		private void OnDisable()
		{
			SocialEngineController.OnLoadTraits -= LoadTraits;
		}

		/// <summary>
		/// Load trait definitions from definition filed provided in the inspector
		/// </summary>
		public void LoadTraits(SocialEngine state)
		{
			var loader = new TraitYamlLoader();

			foreach (var textAsset in m_traitYamlFiles)
			{
				if (textAsset == null) continue;

				var definitions = loader.LoadTraits(textAsset.text);

				foreach (var traitDefinition in definitions)
				{
					state.TraitLibrary.AddTrait(traitDefinition);
				}
			}
		}
	}
}
