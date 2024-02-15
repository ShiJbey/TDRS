using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace TDRS.Serialization
{
	/// <summary>
	/// Manages social engine data that has been restructured for easy serialization.
	/// </summary>
	public class SerializedSocialEngine
	{
		public List<SerializedAgent> agents;

		public List<SerializedRelationship> relationships;

		public SerializedSocialEngine()
		{
			agents = new List<SerializedAgent>();
			relationships = new List<SerializedRelationship>();
		}

		public static string Serialize(SocialEngine socialEngine)
		{
			var serializedEngine = new SerializedSocialEngine();

			foreach (Agent agent in socialEngine.Agents)
			{
				var serializedAgent = new SerializedAgent()
				{
					uid = agent.UID,
					agentType = agent.AgentType
				};

				foreach (TraitInstance trait in agent.Traits.Traits)
				{
					serializedAgent.traits.Add(trait.TraitID);
				}

				foreach (var (statName, stat) in agent.Stats.Stats)
				{
					var serializedStat = new SerializedStat()
					{
						Name = statName,
						BaseValue = stat.BaseValue,
					};

					foreach (var modifier in stat.Modifiers)
					{
						serializedStat.Modifiers.Add(
							new SerializedModifier()
							{
								Value = modifier.Value,
								Order = modifier.Order,
								ModifierType = (int)modifier.ModifierType
							}
						);
					}

					serializedAgent.stats.Add(serializedStat);
				}

				serializedEngine.agents.Add(serializedAgent);
			}

			foreach (Relationship relationship in socialEngine.Relationships)
			{
				var serializedRelationship = new SerializedRelationship()
				{
					owner = relationship.Owner.UID,
					target = relationship.Target.UID,
				};

				foreach (TraitInstance trait in relationship.Traits.Traits)
				{
					serializedRelationship.traits.Add(trait.TraitID);
				}

				foreach (var (statName, stat) in relationship.Stats.Stats)
				{
					var serializedStat = new SerializedStat()
					{
						Name = statName,
						BaseValue = stat.BaseValue,
					};

					foreach (var modifier in stat.Modifiers)
					{
						serializedStat.Modifiers.Add(
							new SerializedModifier()
							{
								Value = modifier.Value,
								Order = modifier.Order,
								ModifierType = (int)modifier.ModifierType
							}
						);
					}

					serializedRelationship.stats.Add(serializedStat);
				}

				serializedEngine.relationships.Add(serializedRelationship);
			}

			var serializer = new SerializerBuilder()
					.JsonCompatible()
					.Build();

			return serializer.Serialize(serializedEngine);
		}

		public static void Deserialize(SocialEngine socialEngine, string dataString)
		{
			var deserializer = new DeserializerBuilder()
					.Build();

			var serializedEngine = deserializer.Deserialize<SerializedSocialEngine>(dataString);

			foreach (var serializedAgent in serializedEngine.agents)
			{
				Agent agent = socialEngine.AddAgent(serializedAgent.agentType, serializedAgent.uid);

				foreach (var traitID in serializedAgent.traits)
				{
					Trait trait = socialEngine.TraitLibrary.Traits[traitID];

					EffectContext ctx = new EffectContext(
						socialEngine,
						"",
						new Dictionary<string, object>()
						{
							{ "?owner", agent.UID },
						},
						trait
					);

					TraitInstance instance = TraitInstance.CreateInstance(socialEngine, trait, ctx, agent);

					agent.Traits.AddTrait(instance);
				}
			}

			foreach (var serializedRelationship in serializedEngine.relationships)
			{
				Relationship relationship = socialEngine.AddRelationship(
					serializedRelationship.owner, serializedRelationship.target);

				foreach (var traitID in serializedRelationship.traits)
				{
					Trait trait = socialEngine.TraitLibrary.Traits[traitID];

					EffectContext ctx = new EffectContext(
						socialEngine,
						"",
						new Dictionary<string, object>()
						{
							{ "?owner", relationship.Owner.UID },
							{ "?target", relationship.Target.UID },
						},
						trait
					);

					TraitInstance instance = TraitInstance.CreateInstance(socialEngine, trait, ctx, relationship);

					relationship.Traits.AddTrait(instance);
				}
			}
		}
	}
}
