using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace TDRS
{
	public class YamlExporter
	{
		#region Constants

		public const int FORMAT_VERSION = 1;

		#endregion

		#region Public Methods

		/// <summary>
		/// Convert the social engine state to a YAML string.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public string ToYaml(SocialEngine state)
		{
			var root = new YamlMappingNode
			{
				{"formatVersion", FORMAT_VERSION.ToString()},
			};

			var agentsNode = new YamlSequenceNode();

			foreach (Agent agent in state.Agents)
			{
				var node = new YamlMappingNode()
				{
					{"uid", agent.UID},
					{"agentType", agent.AgentType}
				};

				var traitsNode = new YamlSequenceNode();
				foreach (Trait trait in agent.Traits.Traits)
				{
					traitsNode.Add(
						new YamlMappingNode()
						{
							{"trait", trait.TraitID},
						}
					);
				}
				node.Add("traits", traitsNode);

				var statsNode = new YamlSequenceNode();
				foreach (var (statName, stat) in agent.Stats.Stats)
				{
					statsNode.Add(
						new YamlMappingNode()
						{
							{"stat", statName},
							{"baseValue", stat.BaseValue.ToString()}
						}
					);
				}
				node.Add("stats", statsNode);

				agentsNode.Add(node);
			}

			root.Add("agents", agentsNode);

			var relationshipsNode = new YamlSequenceNode();

			foreach (Relationship relationship in state.Relationships)
			{
				var node = new YamlMappingNode()
				{
					{"owner", relationship.Owner.UID},
					{"target", relationship.Target.UID},
				};

				var traitsNode = new YamlSequenceNode();
				foreach (Trait trait in relationship.Traits.Traits)
				{
					traitsNode.Add(
						new YamlMappingNode()
						{
							{"trait", trait.TraitID},
						}
					);
				}
				node.Add("traits", traitsNode);

				var statsNode = new YamlSequenceNode();
				foreach (var (statName, stat) in relationship.Stats.Stats)
				{
					statsNode.Add(
						new YamlMappingNode()
						{
							{"stat", statName},
							{"baseValue", stat.BaseValue.ToString()}
						}
					);
				}
				node.Add("stats", statsNode);

				relationshipsNode.Add(node);
			}

			root.Add("relationships", relationshipsNode);

			var serializer = new SerializerBuilder()
				.JsonCompatible()
				.Build();

			string yamlString = serializer.Serialize(root);

			return yamlString;
		}

		#endregion
	}
}
