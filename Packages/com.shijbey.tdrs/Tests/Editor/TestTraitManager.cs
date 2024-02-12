using System;
using NUnit.Framework;

namespace TDRS.Tests
{
	public class TestTraitManager
	{
		[Test]
		public void TestAddTrait()
		{
			var manager = new TraitManager();

			Assert.That(manager.HasTrait("human"), Is.False);

			manager.AddTrait(
				new Trait(
					"human",
					TraitType.Agent,
					"Human",
					"Born a plain ole human."
				)
			);

			Assert.That(manager.HasTrait("human"), Is.True);
		}

		[Test]
		public void TestAddConflictingTrait()
		{
			var manager = new TraitManager();

			bool success;

			success = manager.AddTrait(
				new Trait(
					"human",
					TraitType.Agent,
					"Human",
					"Born a plain ole human."
				)
			);

			Assert.That(success, Is.True);

			success = manager.AddTrait(
				new Trait(
					"vampirism",
					TraitType.Agent,
					"Vampirism",
					"Acquired a slight taste for blood. Oops.",
					new string[0],
					new SocialRule[0],
					new string[] { "human" }
				)
			);

			Assert.That(success, Is.False);
		}

		[Test]
		public void TestAddDuplicateTrait()
		{
			var manager = new TraitManager();

			bool success;

			success = manager.AddTrait(
				new Trait(
					"human",
					TraitType.Agent,
					"Human",
					"Born a plain ole human."
				)
			);

			Assert.That(success, Is.True);

			success = manager.AddTrait(
				new Trait(
					"human",
					TraitType.Agent,
					"Human",
					"Born a plain ole human."
				)
			);

			Assert.That(success, Is.False);
		}

		[Test]
		public void TestRemoveTrait()
		{
			var manager = new TraitManager();

			bool success;

			success = manager.AddTrait(
				new Trait(
					"human",
					TraitType.Agent,
					"Human",
					"Born a plain ole human."
				)
			);

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

			manager.AddTrait(
				new Trait(
					"human",
					TraitType.Agent,
					"Human",
					"Born a plain ole human."
				)
			);

			var vampirism = new Trait(
					"vampirism",
					TraitType.Agent,
					"Vampirism",
					"Acquired a slight taste for blood. Oops.",
					new string[0],
					new SocialRule[0],
					new string[] { "human" }
				);

			Assert.That(manager.HasConflictingTrait(vampirism), Is.True);
		}
	}
}
