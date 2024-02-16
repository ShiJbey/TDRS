using System;
using NUnit.Framework;

namespace TDRS.Tests
{
	public class TestAgent
	{

		/// <summary>
		/// Tolerance threshold for floating point equality assertions.
		/// </summary>
		private double _assertTolerance;

		private SocialEngine _engine;

		[SetUp]
		public void SetUp()
		{
			_assertTolerance = Math.Pow(10, -Stat.ROUND_PRECISION);

			_engine = SocialEngine.Instantiate();

			_engine.TraitLibrary.AddTrait(
				new Trait(
					traitID: "recently-complimented",
					traitType: TraitType.Agent,
					displayName: "Recently complimented",
					description: "[owner] recently received a compliment from someone.",
					modifiers: new StatModifierData[]
					{
						new StatModifierData(
							statName: "Confidence",
							value: 20,
							modifierType: StatModifierType.FLAT
						)
					},
					conflictingTraits: new string[0]
				)
			);

			_engine.TraitLibrary.AddTrait(
				new Trait(
					traitID: "confident",
					traitType: TraitType.Agent,
					displayName: "Confident",
					description: "[owner] is confident.",
					modifiers: new StatModifierData[]
					{
						new StatModifierData(
							statName: "Confidence",
							value: 10,
							modifierType: StatModifierType.FLAT
						)
					},
					conflictingTraits: new string[0]
				)
			);

			_engine.TraitLibrary.AddTrait(
				new Trait(
					traitID: "friendly",
					traitType: TraitType.Agent,
					displayName: "Friendly",
					description: "[owner] is friendly.",
					modifiers: new StatModifierData[0],
					conflictingTraits: new string[0]
				)
			);

			_engine.TraitLibrary.AddTrait(
				new Trait(
					traitID: "attractive",
					traitType: TraitType.Agent,
					displayName: "Attractive",
					description: "[owner] is attractive.",
					modifiers: new StatModifierData[0],
					conflictingTraits: new string[0]
				)
			);

			_engine.AddAgentSchema(
				new AgentSchema(
					"character",
					new StatSchema[]
					{
						new StatSchema(
							statName: "Confidence",
							baseValue: 0,
							maxValue: 50,
							minValue: 0,
							isDiscrete: true
						)
					},
					new string[0]
				)
			);

			_engine.AddRelationshipSchema(
				new RelationshipSchema(
					"character",
					"character",
					new StatSchema[]
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
					},
					new string[0]
				)
			);
		}

		[Test]
		public void TestAddTrait()
		{
			Agent perry = _engine.AddAgent("character", "perry");

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(0));

			perry.AddTrait("confident");

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(10));
		}

		[Test]
		public void TestTraitWithDuration()
		{
			Agent perry = _engine.AddAgent("character", "perry");

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(0));

			perry.AddTrait("recently-complimented", 3);

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(20));

			perry.Tick();
			perry.Tick();
			perry.Tick();
			perry.Tick();

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(0));
		}

		[Test]
		public void TestRemoveTrait()
		{
			Agent perry = _engine.AddAgent("character", "perry");

			perry.AddTrait("confident");

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(10));

			perry.RemoveTrait("confident");

			Assert.That(perry.Stats.GetStat("Confidence").Value, Is.EqualTo(0));
		}
	}
}
