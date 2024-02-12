using NUnit.Framework;

namespace TDRS.Tests
{
	public class TestAgent
	{
		private SocialEngine _state;

		[SetUp]
		public void SetUp()
		{

			_state = SocialEngine.CreateState();

			_state.TraitLibrary.Traits["recently-complimented"] = new Trait(
				traitID: "recently-complimented",
				traitType: TraitType.Agent,
				displayName: "Recently complimented",
				"[owner] recently received a compliment from someone.",
				new string[]
				{
					"IncreaseAgentStat ?owner Confidence 20 3"
				},
				new SocialRule[0],
				new string[0]
			);

			_state.TraitLibrary.Traits["confident"] = new Trait(
				traitID: "confident",
				traitType: TraitType.Agent,
				displayName: "Confident",
				"",
				new string[]
				{
					"IncreaseAgentStat ?owner Confidence 10"
				},
				new SocialRule[0],
				new string[0]
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
