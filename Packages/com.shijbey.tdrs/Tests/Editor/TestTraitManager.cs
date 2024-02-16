using NUnit.Framework;

namespace TDRS.Tests
{
	public class TestTraitManager
	{
		private Trait _humanTrait;
		private Trait _vampirismTrait;
		private SocialEngine _engine;
		private Agent _agent;

		[SetUp]
		public void SetUp()
		{
			_engine = SocialEngine.Instantiate();

			_engine.AddAgentConfig(
				new AgentConfig()
				{
					agentType = "character",
					traits = new string[0],
					stats = new StatSchema[0],
				}
			);

			_agent = _engine.AddAgent("character", "perry");

			_humanTrait = new Trait(
				traitID: "human",
				traitType: TraitType.Agent,
				displayName: "Human",
				description: "",
				modifiers: new StatModifierData[0],
				conflictingTraits: new string[0]
			);

			_vampirismTrait = new Trait(
				traitID: "vampirism",
				traitType: TraitType.Agent,
				displayName: "Vampirism",
				description: "Acquired a slight taste for blood. Oops.",
				modifiers: new StatModifierData[0],
				conflictingTraits: new string[] { "human" }
			);
		}

		[Test]
		public void TestAddTrait()
		{
			TraitManager manager = _agent.Traits;

			Assert.That(manager.HasTrait("human"), Is.False);

			manager.AddTrait(_humanTrait);

			Assert.That(manager.HasTrait("human"), Is.True);
		}

		[Test]
		public void TestAddConflictingTrait()
		{
			TraitManager manager = _agent.Traits;

			bool success;

			success = manager.AddTrait(_humanTrait);

			Assert.That(success, Is.True);

			success = manager.AddTrait(_vampirismTrait);

			Assert.That(success, Is.False);
		}

		[Test]
		public void TestAddDuplicateTrait()
		{
			TraitManager manager = _agent.Traits;

			bool success;

			success = manager.AddTrait(_humanTrait);

			Assert.That(success, Is.True);

			success = manager.AddTrait(_humanTrait);

			Assert.That(success, Is.False);
		}

		[Test]
		public void TestRemoveTrait()
		{
			TraitManager manager = _agent.Traits;

			bool success;

			success = manager.AddTrait(_humanTrait);

			Assert.That(success, Is.True);

			success = manager.RemoveTrait("human");

			Assert.That(success, Is.True);

			success = manager.RemoveTrait("vampirism");

			Assert.That(success, Is.False);
		}

		[Test]
		public void TestHasConflictingTrait()
		{
			TraitManager manager = _agent.Traits;

			manager.AddTrait(_humanTrait);

			Assert.That(manager.HasConflictingTrait(_vampirismTrait), Is.True);
		}
	}
}
