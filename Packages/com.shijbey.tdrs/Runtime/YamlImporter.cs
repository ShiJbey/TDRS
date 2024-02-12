using System.IO;
using TDRS.Helpers;
using YamlDotNet.RepresentationModel;

namespace TDRS
{
	public class YamlImporter
	{
		#region Constants

		public const int MIN_SUPPORTED_FORMAT_VERSION = 1;

		#endregion

		#region Public Methods

		/// <summary>
		/// Load social engine data from YAML and overwrite the given state.
		/// </summary>
		/// <param name="state"></param>
		/// <param name="yamlString"></param>
		public void LoadYaml(SocialEngine state, string yamlString)
		{
			state.Reset();

			var yaml = new YamlStream();

			yaml.Load(new StringReader(yamlString));

			YamlMappingNode root = yaml.Documents[0].RootNode as YamlMappingNode;

			int formatVersion = int.Parse(root.GetChild("formatVersion").GetValue());

			if (formatVersion < MIN_SUPPORTED_FORMAT_VERSION)
			{
				throw new System.Exception("Format version less than minimum supported.");
			}

			YamlSequenceNode agentNodes = root.GetChild("agents") as YamlSequenceNode;

			foreach (YamlNode agentNode in agentNodes)
			{
				string uid = agentNode.GetChild("uid").GetValue();
				string agentType = agentNode.GetChild("agentType").GetValue();

				Agent agent = state.AddAgent(agentType, uid);

				// Set all agent stats to the values in the save file

				YamlSequenceNode savedStats = agentNode.GetChild("stats") as YamlSequenceNode;
				foreach (YamlNode savedStat in savedStats)
				{
					string statName = savedStat.GetChild("stat").GetValue();
					float baseValue = float.Parse(savedStat.GetChild("baseValue").GetValue());

					agent.Stats.GetStat(statName).BaseValue = baseValue;
				}

				// Set all agent traits

				YamlSequenceNode savedTraits = agentNode.GetChild("traits") as YamlSequenceNode;
				foreach (YamlNode savedTrait in savedTraits)
				{
					string traitName = savedTrait.GetChild("trait").GetValue();

					agent.AddTrait(traitName);
				}
			}

			YamlSequenceNode relationshipNodes = root.GetChild("relationships") as YamlSequenceNode;

			foreach (YamlNode relationshipNode in relationshipNodes)
			{
				string ownerId = relationshipNode.GetChild("owner").GetValue();
				string targetId = relationshipNode.GetChild("target").GetValue();

				Relationship relationship = state.AddRelationship(ownerId, targetId);

				// Set all agent stats to the values in the save file

				YamlSequenceNode savedStats = relationshipNode.GetChild("stats") as YamlSequenceNode;
				foreach (YamlNode savedStat in savedStats)
				{
					string statName = savedStat.GetChild("stat").GetValue();
					float baseValue = float.Parse(savedStat.GetChild("baseValue").GetValue());

					relationship.Stats.GetStat(statName).BaseValue = baseValue;
				}

				// Set all agent traits

				YamlSequenceNode savedTraits = relationshipNode.GetChild("traits") as YamlSequenceNode;
				foreach (YamlNode savedTrait in savedTraits)
				{
					string traitName = savedTrait.GetChild("trait").GetValue();

					relationship.AddTrait(traitName);
				}

			}
		}

		#endregion
	}
}
