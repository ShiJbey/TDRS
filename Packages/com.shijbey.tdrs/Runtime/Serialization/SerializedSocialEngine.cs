using System.Collections.Generic;
using System.Linq;
using RePraxis;
using YamlDotNet.Serialization;

namespace TDRS.Serialization
{
	/// <summary>
	/// Manages social engine data that has been restructured for easy serialization.
	/// </summary>
	public class SerializedSocialEngine
	{
		public List<SerializedTrait> traits { get; set; }

		public List<SerializedSocialEvent> socialEvents { get; set; }

		public List<SerializedSocialRule> socialRules { get; set; }

		public List<SerializedAgent> agents { get; set; }

		public List<SerializedRelationship> relationships { get; set; }

		public List<string> dbEntries { get; set; }

		public List<SerializedAgentSchema> agentSchemas { get; set; }

		public List<SerializedRelationshipSchema> relationshipSchemas { get; set; }

		public SerializedSocialEngine()
		{
			traits = new List<SerializedTrait>();
			socialEvents = new List<SerializedSocialEvent>();
			socialRules = new List<SerializedSocialRule>();
			agents = new List<SerializedAgent>();
			relationships = new List<SerializedRelationship>();
			dbEntries = new List<string>();
			agentSchemas = new List<SerializedAgentSchema>();
			relationshipSchemas = new List<SerializedRelationshipSchema>();
		}

		public static string Serialize(SocialEngine socialEngine)
		{
			var serializedEngine = new SerializedSocialEngine();

			foreach (var trait in socialEngine.TraitLibrary.Traits.Values)
			{
				serializedEngine.traits.Add(
					new SerializedTrait()
					{
						traitID = trait.TraitID,
						traitType = trait.TraitType.ToString(),
						displayName = trait.DisplayName,
						description = trait.Description,
						modifiers = trait.Modifiers
							.Select(modifier =>
							{
								return new SerializedStatModifierData()
								{
									statName = modifier.StatName,
									modifierType = modifier.ModifierType.ToString(),
									value = modifier.Value
								};
							}).ToArray(),
						conflictingTraits = trait.ConflictingTraits.ToArray()
					}
				);
			}

			foreach (var entry in socialEngine.SocialEventLibrary.Events.Values)
			{
				serializedEngine.socialEvents.Add(
					new SerializedSocialEvent()
					{
						name = entry.Name,
						roles = entry.Roles,
						description = entry.Description,
						responses = entry.Responses
							.Select(response =>
							{
								return new SerializedSocialEventResponse()
								{
									preconditions = response.Preconditions,
									effects = response.Effects
								};
							}).ToArray()
					}
				);
			}

			foreach (var entry in socialEngine.SocialRules)
			{
				serializedEngine.socialRules.Add(
					new SerializedSocialRule()
					{
						description = entry.Description,
						preconditions = entry.Preconditions,
						modifiers = entry.Modifiers
							.Select(modifier =>
							{
								return new SerializedStatModifierData()
								{
									statName = modifier.StatName,
									modifierType = modifier.ModifierType.ToString(),
									value = modifier.Value
								};
							}).ToArray()
					}
				);
			}

			foreach (Agent agent in socialEngine.Agents)
			{
				var serializedAgent = new SerializedAgent()
				{
					uid = agent.UID,
					agentType = agent.AgentType
				};

				foreach (TraitInstance instance in agent.Traits.Traits)
				{
					serializedAgent.traits.Add(
						new SerializedTraitInstance()
						{
							traitID = instance.TraitID,
							duration = instance.Duration,
							description = instance.Description
						}
					);
				}

				foreach (var (statName, stat) in agent.Stats.Stats)
				{
					var serializedStat = new SerializedStat()
					{
						name = statName,
						baseValue = stat.BaseValue,
					};

					serializedAgent.stats.Add(serializedStat);
				}

				serializedEngine.agents.Add(serializedAgent);
			}

			foreach (Relationship relationship in socialEngine.Relationships)
			{
				var serializedRelationship = new SerializedRelationship()
				{
					owner = relationship.Owner.UID,
					target = relationship.Target.UID
				};

				foreach (TraitInstance instance in relationship.Traits.Traits)
				{
					serializedRelationship.traits.Add(
						new SerializedTraitInstance()
						{
							traitID = instance.TraitID,
							duration = instance.Duration,
							description = instance.Description
						}
					);
				}

				foreach (var (statName, stat) in relationship.Stats.Stats)
				{
					var serializedStat = new SerializedStat()
					{
						name = statName,
						baseValue = stat.BaseValue,
					};

					serializedRelationship.stats.Add(serializedStat);
				}

				serializedEngine.relationships.Add(serializedRelationship);
			}

			// Serialize the database
			serializedEngine.dbEntries = SerializeDatabase(socialEngine.DB);

			foreach (var entry in socialEngine.AgentSchemas.Values)
			{
				serializedEngine.agentSchemas.Add(
					new SerializedAgentSchema()
					{
						agentType = entry.AgentType,
						traits = entry.Traits,
						stats = entry.Stats
							.Select(stat =>
							{
								return new SerializedStatSchema()
								{
									statName = stat.statName,
									baseValue = stat.baseValue,
									minValue = stat.minValue,
									maxValue = stat.maxValue,
									isDiscrete = stat.isDiscrete
								};
							}).ToArray()
					}
				);
			}

			foreach (var entry in socialEngine.RelationshipSchemas.Values)
			{
				serializedEngine.relationshipSchemas.Add(
					new SerializedRelationshipSchema()
					{
						ownerType = entry.OwnerType,
						targetType = entry.TargetType,
						traits = entry.Traits,
						stats = entry.Stats
							.Select(stat =>
							{
								return new SerializedStatSchema()
								{
									statName = stat.statName,
									baseValue = stat.baseValue,
									minValue = stat.minValue,
									maxValue = stat.maxValue,
									isDiscrete = stat.isDiscrete
								};
							}).ToArray()
					}
				);
			}

			// Do the actual serializing
			var serializer = new SerializerBuilder()
					.JsonCompatible()
					.Build();

			return serializer.Serialize(serializedEngine);
		}

		public static SocialEngine Deserialize(string dataString)
		{
			return Deserialize(SocialEngine.Instantiate(), dataString);
		}

		public static SocialEngine Deserialize(SocialEngine socialEngine, string dataString)
		{
			var deserializer = new DeserializerBuilder()
					.Build();

			var serializedEngine = deserializer.Deserialize<SerializedSocialEngine>(dataString);

			foreach (var entry in serializedEngine.traits)
			{
				socialEngine.TraitLibrary.AddTrait(
					entry.ToRuntimeInstance()
				);
			}

			foreach (var entry in serializedEngine.socialEvents)
			{
				socialEngine.SocialEventLibrary.AddSocialEvent(
					entry.ToRuntimeInstance()
				);
			}

			foreach (var entry in serializedEngine.socialRules)
			{
				socialEngine.AddSocialRule(
					new SocialRule(
						description: entry.description,
						preconditions: entry.preconditions,
						modifiers: entry.modifiers
							.Select(modifier =>
							{
								return new StatModifierData(
									statName: modifier.statName,
									value: modifier.value,
									modifierType: StatModifierType.Parse<StatModifierType>(
										modifier.modifierType, true
									)
								);
							})
							.ToArray()
					)
				);
			}

			foreach (var entry in serializedEngine.agentSchemas)
			{
				socialEngine.AddAgentSchema(
					new AgentSchema(
						agentType: entry.agentType,
						stats: entry.stats
							.Select(stat =>
							{
								return new StatSchema(
									statName: stat.statName,
									baseValue: stat.baseValue,
									maxValue: stat.maxValue,
									minValue: stat.minValue,
									isDiscrete: stat.isDiscrete
								);
							}).ToArray(),
						traits: entry.traits
					)
				);
			}

			foreach (var entry in serializedEngine.relationshipSchemas)
			{
				socialEngine.AddRelationshipSchema(
					new RelationshipSchema(
						ownerType: entry.ownerType,
						targetType: entry.targetType,
						stats: entry.stats
							.Select(stat =>
							{
								return new StatSchema(
									statName: stat.statName,
									baseValue: stat.baseValue,
									maxValue: stat.maxValue,
									minValue: stat.minValue,
									isDiscrete: stat.isDiscrete
								);
							}).ToArray(),
						traits: entry.traits
					)
				);
			}

			foreach (var serializedAgent in serializedEngine.agents)
			{
				Agent agent = socialEngine.AddAgent(serializedAgent.agentType, serializedAgent.uid);

				foreach (var entry in serializedAgent.stats)
				{
					agent.Stats.GetStat(entry.name).BaseValue = entry.baseValue;
				}

				foreach (var entry in serializedAgent.traits)
				{
					Trait trait = socialEngine.TraitLibrary.Traits[entry.traitID];

					agent.Traits.AddTrait(trait, entry.description, entry.duration);
				}
			}

			foreach (var serializedRelationship in serializedEngine.relationships)
			{
				Relationship relationship = socialEngine.AddRelationship(
					serializedRelationship.owner, serializedRelationship.target);

				foreach (var entry in serializedRelationship.stats)
				{
					relationship.Stats.GetStat(entry.name).BaseValue = entry.baseValue;
				}

				foreach (var entry in serializedRelationship.traits)
				{
					Trait trait = socialEngine.TraitLibrary.Traits[entry.traitID];

					relationship.Traits.AddTrait(trait, entry.description, entry.duration);
				}
			}

			foreach (var entry in serializedEngine.dbEntries)
			{
				socialEngine.DB.Insert(entry);
			}

			return socialEngine;
		}

		private static List<string> SerializeDatabase(RePraxisDatabase db)
		{
			var entries = new List<string>();

			var nodeStack = new Stack<INode>(db.Root.Children);

			while (nodeStack.Count > 0)
			{
				INode node = nodeStack.Pop();

				IEnumerable<INode> children = node.Children;

				if (children.Count() > 0)
				{
					// Add children to the stack
					foreach (var child in children)
					{
						nodeStack.Push(child);
					}
				}
				else
				{
					// This is a leaf
					entries.Add(node.GetPath());
				}
			}

			return entries;
		}
	}
}
