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
		protected List<TextAsset> m_eventDefinitionFiles;

		/// <summary>
		/// SocialEventFactory instances supplied within the inspector.
		/// </summary>
		[SerializeField]
		protected List<SocialEventEffectFactory> m_effectFactories;

		/// <summary>
		/// Event definitions sorted by name and cardinality.
		/// </summary>
		protected Dictionary<string, SocialEventType> m_eventTypes;

		/// <summary>
		/// Effect names mapped to factories that create instances of that effect.
		/// </summary>
		protected Dictionary<string, ISocialEventEffectFactory> m_effectFactoryDict;

		#endregion

		#region Unity Messages

		private void Awake()
		{
			m_eventTypes = new Dictionary<string, SocialEventType>();
			m_effectFactoryDict = new Dictionary<string, ISocialEventEffectFactory>();
		}

		private void Start()
		{
			LoadFactories();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a factory to the library.
		/// </summary>
		/// <param name="effectType"></param>
		/// <param name="factory"></param>
		public void AddEffectFactory(string effectType, ISocialEventEffectFactory factory)
		{
			m_effectFactoryDict[effectType] = factory;
		}

		/// <summary>
		/// Load social event definitions from text assets provided in the inspector.
		/// </summary>
		public void LoadSocialEvents()
		{
			foreach (var textAsset in m_eventDefinitionFiles)
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

						var eventType = SocialEventType.FromYaml(eventSpecNode);

						m_eventTypes[eventType.ToString()] = eventType;
					}
					catch (KeyNotFoundException)
					{
						Debug.LogError(
							$"Missing 'event' key in entry {i + 1} of {textAsset.name}");
					}
				}
			}
		}

		/// <summary>
		/// Load the various factory instances.
		/// </summary>
		public void LoadFactories()
		{
			foreach (var entry in m_effectFactories)
			{
				AddEffectFactory(entry.EffectType, entry);
			}
		}

		/// <summary>
		/// Get an event type by name (eventName/#)
		/// </summary>
		/// <param name="eventName"></param>
		/// <returns></returns>
		public SocialEventType GetEventType(string eventName)
		{
			return m_eventTypes[eventName];
		}

		/// <summary>
		/// Get an effect factory by event type
		/// </summary>
		/// <param name="effectType"></param>
		/// <returns></returns>
		public ISocialEventEffectFactory GetEffectFactory(string effectType)
		{
			return m_effectFactoryDict[effectType];
		}

		#endregion

		#region Helper Classes

		/// <summary>
		/// Helper class for organizing effect factories in the inspector
		/// </summary>
		[Serializable]
		public class EffectFactoryEntry
		{
			public string m_effectType;
			public SocialEventEffectFactory m_factory;
		}

		#endregion
	}
}
