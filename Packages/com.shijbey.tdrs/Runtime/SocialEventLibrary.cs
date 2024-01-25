using System;
using System.Collections.Generic;
using System.IO;
using TDRS.Helpers;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace TDRS
{
	/// <summary>
	/// Manages the collection of event definitions
	/// </summary>
	public class SocialEventLibrary : MonoBehaviour
	{
		#region Fields

		/// <summary>
		/// A list of text files containing social event definitions.
		/// </summary>
		[SerializeField]
		protected List<TextAsset> m_definitionFiles;

		/// <summary>
		/// Social events defined using ScriptableObjects
		/// </summary>
		[SerializeField]
		protected List<SocialEventSO> m_definitions;

		/// <summary>
		/// Event definitions sorted by name and cardinality.
		/// </summary>
		protected Dictionary<string, SocialEvent> m_eventTypes;

		#endregion

		#region Unity Messages

		private void Awake()
		{
			m_eventTypes = new Dictionary<string, SocialEvent>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Load social event definitions from text assets provided in the inspector.
		/// </summary>
		public void LoadEventDefinitions()
		{
			foreach (var textAsset in m_definitionFiles)
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

						m_eventTypes[eventType.ToString()] = eventType;
					}
					catch (KeyNotFoundException)
					{
						Debug.LogError(
							$"Missing 'event' key in entry {i + 1} of {textAsset.name}");
					}
				}
			}

			// Load social events from scriptable objects
			for (int i = 0; i < m_definitions.Count; i++)
			{
				var socialEvent = m_definitions[i].GetSocialEvent();

				m_eventTypes[socialEvent.ToString()] = socialEvent;
			}
		}

		/// <summary>
		/// Get an event type by name (eventName/#)
		/// </summary>
		/// <param name="eventName"></param>
		/// <returns></returns>
		public SocialEvent GetEventType(string eventName)
		{
			return m_eventTypes[eventName];
		}

		#endregion
	}
}
