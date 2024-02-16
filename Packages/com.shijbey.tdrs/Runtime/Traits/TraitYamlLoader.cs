using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;
using TDRS.Helpers;
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
		/// <param name="yamlNode"></param>
		/// <returns></returns>
		public Trait LoadTrait(YamlNode yamlNode)
		{
			var deserializer = new DeserializerBuilder().Build();

			SerializedTrait serializedTrait = deserializer.Deserialize<SerializedTrait>(
				yamlNode.ToString());

			Trait trait = serializedTrait.ToRuntimeInstance();

			return trait;
		}

		/// <summary>
		/// Load a single trait.
		/// </summary>
		/// <param name="yamlString"></param>
		/// <returns></returns>
		public Trait LoadTrait(string yamlString)
		{
			// var yaml = new YamlStream();

			// yaml.Load(new StringReader(yamlString));

			// YamlNode root = yaml.Documents[0].RootNode;

			// if (root.NodeType != YamlNodeType.Mapping)
			// {
			// 	throw new System.Exception("LoadTrait expected a YAML mapping/object.");
			// }

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
			// List<Trait> traits = new List<Trait>();

			// var yaml = new YamlStream();

			// yaml.Load(new StringReader(yamlString));

			// YamlNode root = yaml.Documents[0].RootNode;

			// if (root.NodeType != YamlNodeType.Sequence)
			// {
			// 	throw new System.Exception("LoadTraits expected a YAML sequence/list.");
			// }

			// YamlSequenceNode rootSequence = root as YamlSequenceNode;

			// foreach (var child in rootSequence)
			// {
			// 	traits.Add(LoadTrait(child));
			// }

			// return traits;

			var deserializer = new DeserializerBuilder().Build();

			List<Trait> traits =
				deserializer.Deserialize<List<SerializedTrait>>(yamlString)
				.Select(s => { return s.ToRuntimeInstance(); })
				.ToList();

			return traits;
		}
	}
}
