using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TDRS.Tests
{
	public class TestEffects
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
		}

		public void TestAddAgentTrait()
		{
			// Agent perry = _engine.AddAgent("character", "perry");

			// EffectContext context = new EffectContext(
			// 	_engine,
			// 	_engine.TraitLibrary.Traits["confident"].Description,
			// 	new Dictionary<string, object>()
			// 	{
			// 		{"?owner", "perry"}
			// 	},
			// 	_engine.TraitLibrary.Traits["confident"]
			// );

			// AddAgentTrait effect = new AddAgentTrait(context, perry, "confident", -1);

			// effect.Apply();

			// Assert.That(
			// 	perry.Effects.Effects.Count,
			// 	Is.EqualTo(2)
			// );

			// Assert.That(
			// 	perry.Stats.GetStat("Confidence").Value,
			// 	Is.EqualTo(10).Within(_assertTolerance)
			// );

			// effect.Remove();

			// Assert.That(
			// 	perry.Stats.GetStat("Confidence").Value,
			// 	Is.EqualTo(0).Within(_assertTolerance)
			// );
		}

		public void TestAddAgentTraitTemporary()
		{
			// Agent perry = _engine.AddAgent("character", "perry");

			// EffectContext context = new EffectContext(
			// 	_engine,
			// 	_engine.TraitLibrary.Traits["confident"].Description,
			// 	new Dictionary<string, object>()
			// 	{
			// 		{"?owner", "perry"}
			// 	},
			// 	_engine.TraitLibrary.Traits["confident"]
			// );

			// AddAgentTrait effect = new AddAgentTrait(context, perry, "confident", 2);

			// effect.Apply();

			// Assert.That(
			// 	perry.Effects.Effects.Count,
			// 	Is.EqualTo(2)
			// );

			// Assert.That(
			// 	perry.Stats.GetStat("Confidence").Value,
			// 	Is.EqualTo(10).Within(_assertTolerance)
			// );

			// _engine.Tick();
			// _engine.Tick();

			// Assert.That(
			// 	perry.Stats.GetStat("Confidence").Value,
			// 	Is.EqualTo(0).Within(_assertTolerance)
			// );
		}

		public void TestAddRelationshipTrait()
		{
			// TODO
		}

		public void TestAddRelationshipTraitTemporary()
		{
			// TODO
		}
	}
}
