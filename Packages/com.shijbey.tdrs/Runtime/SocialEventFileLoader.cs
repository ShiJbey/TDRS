using System.Collections.Generic;
using System.IO;
using TDRS.Helpers;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace TDRS
{
	/// <summary>
	/// Loads social event definitions from a given path or from a text asset
	/// </summary>
	public class SocialEventFileLoader : MonoBehaviour
	{
		[SerializeField]
		private TextAsset[] m_socialEvents;

		private void OnEnable()
		{
			SocialEngine.OnLoadSocialEvents += LoadSocialEvents;
		}

		private void OnDisable()
		{
			SocialEngine.OnLoadSocialEvents -= LoadSocialEvents;
		}

		/// <summary>
		/// Load social event definitions from text assets provided in the inspector.
		/// </summary>
		private void LoadSocialEvents(SocialEngineState state)
		{
			foreach (var textAsset in m_socialEvents)
			{
				var input = new StringReader(textAsset.text);

				var yaml = new YamlStream();
				yaml.Load(input);

				var root = (YamlSequenceNode)yaml.Documents[0].RootNode;
				for (int i = 0; i < root.Children.Count; i++)
				{
					var eventSpecNode = root.Children[i];

					try
					{
						var eventKeyNode = eventSpecNode.GetChild("event");

						var eventType = SocialEvent.FromYaml(eventSpecNode);

						state.SocialEventLibrary.AddSocialEvent(eventType);
					}
					catch (KeyNotFoundException)
					{
						Debug.LogError(
							$"Missing 'event' key in entry {i + 1} of {textAsset.name}");
					}
				}
			}
		}
	}
}
