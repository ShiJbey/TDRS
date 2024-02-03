using System.IO;
using YamlDotNet.RepresentationModel;
using UnityEngine;

namespace TDRS
{
	public class TraitFileLoader : MonoBehaviour
	{
		/// <summary>
		/// A list of TextAssets assigned within the Unity inspector
		/// </summary>
		[SerializeField]
		protected TextAsset[] m_definitionFiles;

		private void OnEnable()
		{
			SocialEngine.OnLoadTraits += LoadTraitDefinitions;
		}

		private void OnDisable()
		{
			SocialEngine.OnLoadTraits -= LoadTraitDefinitions;
		}


		/// <summary>
		/// Load trait definitions from definition filed provided in the inspector
		/// </summary>
		public void LoadTraitDefinitions(SocialEngineState state)
		{
			foreach (var textAsset in m_definitionFiles)
			{
				var input = new StringReader(textAsset.text);

				var yaml = new YamlStream();
				yaml.Load(input);

				var rootMapping = (YamlSequenceNode)yaml.Documents[0].RootNode;

				foreach (var node in rootMapping.Children)
				{
					state.TraitLibrary.AddTraitDefinition(TraitDefinition.FromYaml(node));
				}
			}
		}
	}
}
