using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;
using RePraxis;
using TDRS.Helpers;

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
			string traitID = yamlNode.GetChild("traitID").GetValue();

			TraitType traitType = TraitType.Parse<TraitType>(
					yamlNode.GetChild("traitType").GetValue(), true);

			string displayName = yamlNode.GetChild("displayName").GetValue();

			string description = yamlNode.GetChild("description").GetValue();

			string[] effects = new string[0];

			SocialRule[] socialRules = new SocialRule[0];

			HashSet<string> conflictingTraits = new HashSet<string>();

			// Attempt to set the effects
			if (yamlNode.TryGetChild("effects", out var effectsNode))
			{
				effects = (effectsNode as YamlSequenceNode).Children
					.Select(node => node.GetValue())
					.ToArray();
			}

			// Attempt to set social rules
			if (yamlNode.TryGetChild("socialRules", out var socialRulesNode))
			{
				socialRules = (socialRulesNode as YamlSequenceNode).Children
					.Select(node => LoadSocialRule(node, description))
					.ToArray();
			}

			// Attempt to set conflicting traits
			if (yamlNode.TryGetChild("conflictingTraits", out var conflictingTraitsNode))
			{
				conflictingTraits = new HashSet<string>(
					(conflictingTraitsNode as YamlSequenceNode).Children
						.Select(node => node.GetValue())
				);
			}

			return new Trait(
				traitID,
				traitType,
				displayName,
				description,
				effects,
				socialRules,
				conflictingTraits
			);
		}

		/// <summary>
		/// Load a single trait.
		/// </summary>
		/// <param name="yamlString"></param>
		/// <returns></returns>
		public Trait LoadTrait(string yamlString)
		{
			var yaml = new YamlStream();

			yaml.Load(new StringReader(yamlString));

			YamlNode root = yaml.Documents[0].RootNode;

			if (root.NodeType != YamlNodeType.Mapping)
			{
				throw new System.Exception("LoadTrait expected a YAML mapping/object.");
			}

			return LoadTrait(root);
		}

		/// <summary>
		/// Load a list of traits.
		/// </summary>
		/// <param name="yamlString"></param>
		/// <returns></returns>
		public List<Trait> LoadTraits(string yamlString)
		{
			List<Trait> traits = new List<Trait>();

			var yaml = new YamlStream();

			yaml.Load(new StringReader(yamlString));

			YamlNode root = yaml.Documents[0].RootNode;

			if (root.NodeType != YamlNodeType.Sequence)
			{
				throw new System.Exception("LoadTraits expected a YAML sequence/list.");
			}

			YamlSequenceNode rootSequence = root as YamlSequenceNode;

			foreach (var child in rootSequence)
			{
				traits.Add(LoadTrait(child));
			}

			return traits;
		}

		/// <summary>
		/// Load a social rule for a trait.
		/// </summary>
		/// <param name="yamlNode"></param>
		/// <returns></returns>
		private SocialRule LoadSocialRule(
			YamlNode yamlNode,
			string traitDescription
		)
		{
			string description = traitDescription;
			string[] effects = new string[0];
			DBQuery precondition = new DBQuery();

			// Try to set the query
			if (yamlNode.TryGetChild("precondition", out var preconditionNode))
			{
				precondition = new DBQuery(
					preconditionNode.GetValue()
						.Split("\n")
						.Where(clause => clause != "")
						.ToArray()
				);
			}

			// Try to set the effects
			if (yamlNode.TryGetChild("effects", out var effectsNode))
			{
				effects = (effectsNode as YamlSequenceNode).Children
					.Select(node => node.GetValue())
					.ToArray();
			}
			else
			{
				throw new System.ArgumentException(
					"Social rule definition is missing 'effects' section");
			}

			if (yamlNode.TryGetChild("description", out var descriptionNode))
			{
				description = descriptionNode.GetValue();
			}

			return new SocialRule(precondition, effects, description);
		}
	}
}
