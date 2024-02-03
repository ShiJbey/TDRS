using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace TDRS
{
	public class JsonExporter
	{
		#region Constants

		public const int FORMAT_VERSION = 1;

		#endregion

		#region Public Methods

		/// <summary>
		/// Convert the social engine state to a JSON string.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public string ToJson(SocialEngineState state)
		{
			var root = new YamlMappingNode
			{
				{"formatVersion", FORMAT_VERSION.ToString()},
			};

			var agentsNode = new YamlSequenceNode();

			foreach (AgentNode agent in state.Agents)
			{
				var node = new YamlMappingNode()
				{
					{"uid", agent.UID},
					{"agentType", agent.NodeType}
				};

				var traitsNode = new YamlSequenceNode();
				foreach (Trait trait in agent.Traits.Traits)
				{
					traitsNode.Add(
						new YamlMappingNode()
						{
							{"trait", trait.TraitID},
							{"duration", trait.Duration.ToString()}
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

			foreach (RelationshipEdge relationship in state.Relationships)
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
							{"duration", trait.Duration.ToString()}
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

			string jsonString = serializer.Serialize(root);

			return jsonString;
		}

		#endregion
	}
}
