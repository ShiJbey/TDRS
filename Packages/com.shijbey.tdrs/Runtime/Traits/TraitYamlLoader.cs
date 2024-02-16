using System.Collections.Generic;
using System.Linq;
using TDRS.Serialization;
using YamlDotNet.Serialization;

namespace TDRS
{
	/// <summary>
	/// Loads trait definitions from YAML data.
	/// </summary>
	public class TraitYamlLoader
	{
		/// <summary>
		/// Load a single trait.
		/// </summary>
		/// <param name="yamlString"></param>
		/// <returns></returns>
		public Trait LoadTrait(string yamlString)
		{
			var deserializer = new DeserializerBuilder().Build();

			SerializedTrait serializedTrait = deserializer.Deserialize<SerializedTrait>(yamlString);

			Trait trait = serializedTrait.ToRuntimeInstance();

			return trait;
		}

		/// <summary>
		/// Load a list of traits.
		/// </summary>
		/// <param name="yamlString"></param>
		/// <returns></returns>
		public List<Trait> LoadTraits(string yamlString)
		{
			var deserializer = new DeserializerBuilder().Build();

			List<Trait> traits =
				deserializer.Deserialize<List<SerializedTrait>>(yamlString)
				.Select(s => { return s.ToRuntimeInstance(); })
				.ToList();

			return traits;
		}
	}
}
