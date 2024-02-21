using System.Collections.Generic;
using System.Linq;
using TDRS.Serialization;
using YamlDotNet.Serialization;

namespace TDRS
{
	/// <summary>
	/// Load social event information from YAML files.
	/// </summary>
	public class SocialEventYamlLoader
	{
		/// <summary>
		/// Load a single social event from a yaml mapping.
		/// </summary>
		/// <param name="yamlString"></param>
		/// <returns></returns>
		public SocialEvent LoadSocialEvent(string yamlString)
		{
			var deserializer = new DeserializerBuilder().Build();

			SocialEvent socialEvent = deserializer
				.Deserialize<SerializedSocialEvent>(yamlString)
				.ToRuntimeInstance();

			return socialEvent;
		}

		/// <summary>
		/// Load multiple social events from a yaml sequence.
		/// </summary>
		/// <param name="yamlString"></param>
		/// <returns></returns>
		public List<SocialEvent> LoadSocialEvents(string yamlString)
		{
			var deserializer = new DeserializerBuilder().Build();

			List<SocialEvent> socialEvent = deserializer
				.Deserialize<List<SerializedSocialEvent>>(yamlString)
				.Select(e => { return e.ToRuntimeInstance(); })
				.ToList();

			return socialEvent;
		}
	}
}
