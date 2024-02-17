using System.Collections.Generic;
using System.Linq;
using TDRS.Serialization;
using YamlDotNet.Serialization;

namespace TDRS
{
	public class SocialRuleYamlLoader
	{
		/// <summary>
		/// Load a single social rule.
		/// </summary>
		/// <param name="yamlString"></param>
		/// <returns></returns>
		public SocialRule LoadSocialRule(string yamlString)
		{
			var deserializer = new DeserializerBuilder().Build();

			SocialRule socialRule =
				deserializer.Deserialize<SerializedSocialRule>(yamlString)
				.ToRuntimeInstance();

			return socialRule;
		}

		/// <summary>
		/// Load a list of social rules.
		/// </summary>
		/// <param name="yamlString"></param>
		/// <returns></returns>
		public List<SocialRule> LoadSocialRules(string yamlString)
		{
			var deserializer = new DeserializerBuilder().Build();

			List<SocialRule> socialRules =
				deserializer.Deserialize<List<SerializedSocialRule>>(yamlString)
					.Select(s => { return s.ToRuntimeInstance(); })
					.ToList();

			return socialRules;
		}
	}
}
