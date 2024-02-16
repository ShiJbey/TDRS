using System;
using NUnit.Framework;

namespace TDRS.Tests
{
	public class TestSocialRules
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

			_engine.AddSocialRule(
				new SocialRule(
					ruleID: "people_like_attractiveness",
					description: "Characters like attractive characters",
					preconditions: new string[]
					{
						"?other.traits.attractive"
					},
					modifiers: new StatModifierData[]
					{
						new StatModifierData(
							statName: "Romance",
							value: 12,
							modifierType: StatModifierType.FLAT
						)
					}
				)
			);

			_engine.AddSocialRule(
				new SocialRule(
					ruleID: "friendly_characters_make_friends",
					description: "Friendly characters are more friendly",
					preconditions: new string[]
					{
						"?owner.traits.friendly",
					},
					modifiers: new StatModifierData[]
					{
						new StatModifierData(
							statName: "Friendship",
							value: 10,
							modifierType: StatModifierType.FLAT
						)
					}
				)
			);
		}

		[Test]
		public void TestOutgoingSocialRule()
		{
			Agent liza = _engine.AddAgent("character", "liza");

			Agent zim = _engine.AddAgent("character", "zim");

			var liza_to_zim = _engine.AddRelationship(liza, zim);

			Assert.That(
				liza_to_zim.Stats.GetStat("Friendship").Value,
				Is.EqualTo(0).Within(_assertTolerance));

			liza.AddTrait("friendly");

			Assert.That(
				liza_to_zim.Stats.GetStat("Friendship").Value,
				Is.EqualTo(10).Within(_assertTolerance));

			liza.RemoveTrait("friendly");

			Assert.That(
				liza_to_zim.Stats.GetStat("Friendship").Value,
				Is.EqualTo(0).Within(_assertTolerance));
		}

		[Test]
		public void TestIncomingSocialRule()
		{
			Agent liza = _engine.AddAgent("character", "liza");

			Agent zim = _engine.AddAgent("character", "zim");

			var liza_to_zim = _engine.AddRelationship(liza, zim);

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

	}
}
