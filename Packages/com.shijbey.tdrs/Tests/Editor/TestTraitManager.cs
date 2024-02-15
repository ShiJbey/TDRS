using System.Collections.Generic;
using NUnit.Framework;

namespace TDRS.Tests
{
	public class TestTraitManager
	{
		private Trait _humanTrait;
		private Trait _vampirismTrait;
		private TraitInstance _humanTraitInstance;
		private TraitInstance _vampirismTraitInstance;
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
				displayName: "Human"
			);

			_humanTraitInstance = new TraitInstance(
				_agent,
				_humanTrait,
				"",
				new List<IEffect>()
			);

			_vampirismTrait = new Trait(
					"vampirism",
					TraitType.Agent,
					"Vampirism"
				)
			{
				Description = "Acquired a slight taste for blood. Oops.",
				ConflictingTraits = new HashSet<string>() { "human" }
			};

			_vampirismTraitInstance = new TraitInstance(
				_agent,
				_vampirismTrait,
				"",
				new List<IEffect>()
			);
		}

		[Test]
		public void TestAddTrait()
		{
			var manager = new TraitManager();

			Assert.That(manager.HasTrait("human"), Is.False);

			manager.AddTrait(_humanTraitInstance);

			Assert.That(manager.HasTrait("human"), Is.True);
		}

		[Test]
		public void TestAddConflictingTrait()
		{
			var manager = new TraitManager();

			bool success;

			success = manager.AddTrait(_humanTraitInstance);

			Assert.That(success, Is.True);

			success = manager.AddTrait(_vampirismTraitInstance);

			Assert.That(success, Is.False);
		}

		[Test]
		public void TestAddDuplicateTrait()
		{
			var manager = new TraitManager();

			bool success;

			success = manager.AddTrait(_humanTraitInstance);

			Assert.That(success, Is.True);

			success = manager.AddTrait(_humanTraitInstance);

			Assert.That(success, Is.False);
		}

		[Test]
		public void TestRemoveTrait()
		{
			var manager = new TraitManager();

			bool success;

			success = manager.AddTrait(_humanTraitInstance);

			Assert.That(success, Is.True);

			success = manager.RemoveTrait("human");

			Assert.That(success, Is.True);

			success = manager.RemoveTrait("vampirism");

			Assert.That(success, Is.False);
		}

		[Test]
		public void TestHasConflictingTrait()
		{
			var manager = new TraitManager();

			manager.AddTrait(_humanTraitInstance);

			Assert.That(manager.HasConflictingTrait(_vampirismTrait), Is.True);
		}
	}
}
