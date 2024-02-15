using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TDRS.Tests
{
	public class TestAgent
	{

		/// <summary>
		/// Tolerance threshold for floating point equality assertions.
		/// </summary>
		private double _assertTolerance;

		private SocialEngine _state;

		[SetUp]
		public void SetUp()
		{
			_assertTolerance = Math.Pow(10, -Stat.ROUND_PRECISION);

			_state = SocialEngine.Instantiate();

			_state.TraitLibrary.AddTrait(
				new Trait(
					traitID: "recently-complimented",
					traitType: TraitType.Agent,
					displayName: "Recently complimented"
				)
				{
					Description = "[owner] recently received a compliment from someone.",
					Effects = new List<string>()
					{
						"IncreaseAgentStat ?owner Confidence 20 3"
					},
				}
			);

			_state.TraitLibrary.AddTrait(
				new Trait(
					traitID: "confident",
					traitType: TraitType.Agent,
					displayName: "Confident"
				)
				{
					Description = "[owner] is confident.",
					Effects = new List<string>()
					{
						"IncreaseAgentStat ?owner Confidence 10"
					}
				}
			);

			_state.TraitLibrary.AddTrait(
				new Trait(
					traitID: "friendly",
					traitType: TraitType.Agent,
					displayName: "Friendly"
				)
				{
					Description = "[owner] is friendly.",
					SocialRules = new List<SocialRule>()
					{
						new SocialRule()
						{
							DescriptionTemplate = "[owner] is friendly",
							Preconditions = new string[]
							{
								"?owner.relationships.?other"
							},
							Effects = new string[]
							{
								"IncreaseRelationshipStat ?owner ?other Friendship 12"
							}
						}
					}
				}
			);

			_state.TraitLibrary.AddTrait(
				new Trait(
				traitID: "attractive",
				traitType: TraitType.Agent,
				displayName: "Attractive"
				)
				{
					Description = "[owner] is attractive.",
					SocialRules = new List<SocialRule>()
					{
						new SocialRule()
						{
							DescriptionTemplate = "[owner] is attractive",
							Preconditions = new string[]
							{
								"?other.relationships.?owner",
								"?owner.traits.attractive"
							},
							Effects = new string[]
							{
								"IncreaseRelationshipStat ?other ?owner Romance 12"
							}
						}
					},
				}
			);

			_state.AgentConfigs["character"] = new AgentConfig()
			{
				agentType = "character",
				traits = new string[0],
				stats = new StatSchema[]
				{
					new StatSchema(
						statName: "Confidence",
						baseValue: 0,
						maxValue: 50,
						minValue: 0,
						isDiscrete: true
					)
				}
			};

			_state.RelationshipConfigs[("character", "character")] = new RelationshipConfig()
			{
				ownerAgentType = "character",
				targetAgentType = "character",
				traits = new string[0],
				stats = new StatSchema[]
				{
					new StatSchema(
						statName: "Friendship",
						baseValue: 0,
						maxValue: 50,
						minValue: 0,
						isDiscrete: true
					),
					new StatSchema(
						statName: "Romance",
						baseValue: 0,
						maxValue: 50,
						minValue: 0,
						isDiscrete: true
					)
				}
			};
		}

		[Test]
		public void TestAddTrait()
		{
			Agent perry = _state.AddAgent("character", "perry");

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(0));

			perry.AddTrait("confident");

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(10));
		}

		[Test]
		public void TestTraitWithTemporaryEffect()
		{
			Agent perry = _state.AddAgent("character", "perry");

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(0));

			perry.AddTrait("recently-complimented");

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(20));

			perry.Tick();
			perry.Tick();
			perry.Tick();
			perry.Tick();

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(0));
		}

		[Test]
		public void TestAddTraitWithOutgoingSocialRule()
		{
			Agent liza = _state.AddAgent("character", "liza");

			Agent zim = _state.AddAgent("character", "zim");

			var liza_to_zim = _state.AddRelationship(liza, zim);

			Assert.That(
				liza_to_zim.Stats.GetStat("Friendship").Value,
				Is.EqualTo(0).Within(_assertTolerance));

			liza.AddTrait("friendly");

			Assert.That(
				liza_to_zim.Stats.GetStat("Friendship").Value,
				Is.EqualTo(12).Within(_assertTolerance));

			Agent perry = _state.AddAgent("character", "perry");

			var liza_to_perry = _state.AddRelationship(liza, perry);

			Assert.That(
				liza_to_perry.Stats.GetStat("Friendship").Value,
				Is.EqualTo(12).Within(_assertTolerance));
		}

		[Test]
		public void TestRemoveTraitWithOutgoingSocialRule()
		{
			Agent liza = _state.AddAgent("character", "liza");

			Agent zim = _state.AddAgent("character", "zim");

			var liza_to_zim = _state.AddRelationship(liza, zim);

			Assert.That(
				liza_to_zim.Stats.GetStat("Friendship").Value,
				Is.EqualTo(0).Within(_assertTolerance));

			liza.AddTrait("friendly");

			Assert.That(
				liza_to_zim.Stats.GetStat("Friendship").Value,
				Is.EqualTo(12).Within(_assertTolerance));

			liza.RemoveTrait("friendly");

			Assert.That(
				liza_to_zim.Stats.GetStat("Friendship").Value,
				Is.EqualTo(0).Within(_assertTolerance));
		}

		[Test]
		public void TestAddTraitWithIncomingSocialRule()
		{
			Agent liza = _state.AddAgent("character", "liza");

			Agent zim = _state.AddAgent("character", "zim");

			var liza_to_zim = _state.AddRelationship(liza, zim);

			Assert.That(
				liza_to_zim.Stats.GetStat("Romance").Value,
				Is.EqualTo(0).Within(_assertTolerance));

			zim.AddTrait("attractive");

			Assert.That(
				liza_to_zim.Stats.GetStat("Romance").Value,
				Is.EqualTo(12).Within(_assertTolerance));

			Agent perry = _state.AddAgent("character", "perry");

			var perry_to_zim = _state.AddRelationship(perry, zim);

			Assert.That(
				perry_to_zim.Stats.GetStat("Romance").Value,
				Is.EqualTo(12).Within(_assertTolerance));
		}

		[Test]
		public void TestRemoveTraitWithIncomingSocialRule()
		{
			Agent liza = _state.AddAgent("character", "liza");

			Agent zim = _state.AddAgent("character", "zim");

			var liza_to_zim = _state.AddRelationship(liza, zim);

			Assert.That(
				liza_to_zim.Stats.GetStat("Romance").Value,
				Is.EqualTo(0).Within(_assertTolerance));

			zim.AddTrait("attractive");

			Assert.That(
				liza_to_zim.Stats.GetStat("Romance").Value,
				Is.EqualTo(12).Within(_assertTolerance));

			zim.RemoveTrait("attractive");

			Assert.That(
				liza_to_zim.Stats.GetStat("Romance").Value,
				Is.EqualTo(0).Within(_assertTolerance));
		}

		[Test]
		public void TestRemoveTrait()
		{
			Agent perry = _state.AddAgent("character", "perry");

			perry.AddTrait("confident");

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(10));

			perry.RemoveTrait("confident");

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(0));
		}
	}
}
