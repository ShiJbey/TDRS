using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace TDRS
{
	/// <summary>
	/// Loads social event definitions from a given path or from a text asset
	/// </summary>
	public class SocialEventFileLoader : MonoBehaviour
	{
		/// <summary>
		/// A list of file paths relative to the StreamingAssets directory
		/// </summary>
		[SerializeField]
		[Tooltip("Loaded from StreamingAssets directory")]
		private string[] m_filePaths;

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
			foreach (var path in m_filePaths)
			{
				string filePath = Path.Combine(Application.streamingAssetsPath, path);

				var input = new StringReader(File.ReadAllText(filePath));

				var yaml = new YamlStream();
				yaml.Load(input);

				var root = (YamlSequenceNode)yaml.Documents[0].RootNode;
				for (int i = 0; i < root.Children.Count; i++)
				{
					var eventSpecNode = root.Children[i];

					try
					{
						var eventType = SocialEvent.FromYaml(eventSpecNode);

						state.SocialEventLibrary.AddSocialEvent(eventType);
					}
					catch (KeyNotFoundException)
					{
						Debug.LogError(
							$"Missing 'event' key in entry {i + 1} of {path}");
					}
				}
			}
		}
	}
}
