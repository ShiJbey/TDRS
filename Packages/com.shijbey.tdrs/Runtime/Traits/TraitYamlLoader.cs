using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;
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
			Trait trait = new Trait(
				yamlNode.GetChild("traitID").GetValue(),
				TraitType.Parse<TraitType>(
					yamlNode.GetChild("traitType").GetValue(), true),
				yamlNode.GetChild("displayName").GetValue()
			);

			if (yamlNode.TryGetChild("description", out var descriptionNode))
			{
				trait.Description = descriptionNode.GetValue();
			}

			// Attempt to set the effects
			if (yamlNode.TryGetChild("effects", out var effectsNode))
			{
				trait.Effects = (effectsNode as YamlSequenceNode).Children
					.Select(node => node.GetValue())
					.ToList();
			}

			// Attempt to set social rules
			if (yamlNode.TryGetChild("socialRules", out var socialRulesNode))
			{
				trait.SocialRules = (socialRulesNode as YamlSequenceNode).Children
					.Select(node => LoadSocialRule(node, trait.Description))
					.ToList();
			}

			// Attempt to set conflicting traits
			if (yamlNode.TryGetChild("conflictingTraits", out var conflictingTraitsNode))
			{
				trait.ConflictingTraits = new HashSet<string>(
					(conflictingTraitsNode as YamlSequenceNode).Children
						.Select(node => node.GetValue())
				);
			}

			return trait;
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
			SocialRule socialRule = new SocialRule()
			{
				DescriptionTemplate = traitDescription
			};

			// Try to set the query
			if (yamlNode.TryGetChild("preconditions", out var preconditionNode))
			{
				socialRule.Preconditions = (preconditionNode as YamlSequenceNode).Children
					.Select(child => child.GetValue())
					.ToArray();
			}

			// Try to set the effects
			if (yamlNode.TryGetChild("effects", out var effectsNode))
			{
				socialRule.Effects = (effectsNode as YamlSequenceNode).Children
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
				socialRule.DescriptionTemplate = descriptionNode.GetValue();
			}

			return socialRule;
		}
	}
}
