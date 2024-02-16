using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// Loads social event definitions from a given path or from a text asset
	/// </summary>
	public class SocialEventYamlFileLoader : MonoBehaviour
	{
		/// <summary>
		/// YAML files containing social events to load
		/// </summary>
		[SerializeField]
		private TextAsset[] m_eventYamlFiles;

		private void OnEnable()
		{
			SocialEngineController.OnLoadSocialEvents += LoadSocialEvents;
		}

		private void OnDisable()
		{
			SocialEngineController.OnLoadSocialEvents -= LoadSocialEvents;
		}

		/// <summary>
		/// Load social event definitions from text assets provided in the inspector.
		/// </summary>
		private void LoadSocialEvents(SocialEngine state)
		{

			var loader = new SocialEventYamlLoader();

			foreach (var textAsset in m_eventYamlFiles)
			{
				if (textAsset == null) continue;

				var definitions = loader.LoadSocialEvents(textAsset.text);

				foreach (var eventDefinition in definitions)
				{
					state.SocialEventLibrary.AddSocialEvent(eventDefinition);
				}
			}
		}
	}
}
