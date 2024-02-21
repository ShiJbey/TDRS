using System;
using NUnit.Framework;

namespace TDRS.Tests
{
	/// <summary>
	/// Test the StatManager class and associated methods.
	/// </summary>
	public class TestStatManager
	{
		/// <summary>
		/// Plays the role of an example object that might subscribe to a stat.
		/// </summary>
		private class MockStatObserver
		{
			private string statToWatch;
			public float Value { get; private set; }

			public MockStatObserver()
			{
				statToWatch = "";
				Value = 0f;
			}

			public void Observe(StatManager manager, string stat)
			{
				if (statToWatch != "")
				{
					throw new Exception("Cannot observe more than one variable.");
				}

				statToWatch = stat;

				if (manager.HasStat(stat))
				{
					Value = manager.GetStat(stat).Value;
				}

				manager.OnValueChanged += HandleUpdate;
			}

			public void StopObserve(StatManager manager)
			{
				manager.OnValueChanged -= HandleUpdate;
				statToWatch = "";
			}

			private void HandleUpdate(object obj, StatManager.OnValueChangedArgs args)
			{
				if (args.StatName == statToWatch)
				{
					Value = args.Value;
				}
			}
		}

		/// <summary>
		/// A placeholder source for stat used in unity tests.
		/// </summary>
		private class MockStatModifierSource { }

		/// <summary>
		/// Tolerance threshold for floating point equality assertions.
		/// </summary>
		private double assertTolerance;

		[SetUp]
		public void SetUp()
		{
			assertTolerance = Math.Pow(10, -Stat.ROUND_PRECISION);
		}

		[Test]
		public void TestAddStat()
		{
			var manager = new StatManager();

			var observer = new MockStatObserver();

			observer.Observe(manager, "mana");

			manager.AddStat("mana", new Stat(0, 0, 100, true));

			Assert.That(manager.HasStat("mana"), Is.True);
		}

		/// <summary>
		/// Test Getting
		/// </summary>
		[Test]
		public void TestGetStat()
		{
			var manager = new StatManager();

			manager.AddStat("mana", new Stat(0, 0, 100, true));

			var stat = manager.GetStat("mana");

			Assert.That(stat.Value, Is.EqualTo(0));
		}

		/// <summary>
		/// Ensure OnValueAdded events are invoked when modifiers are added to stats.
		/// </summary>
		[Test]
		public void TestAddModifier()
		{
			var manager = new StatManager();

			var observer = new MockStatObserver();

			observer.Observe(manager, "mana");

			manager.AddStat("mana", new Stat(25, 0, 100, true));

			manager.GetStat("mana").AddModifier(new StatModifier(-5, StatModifierType.FLAT, -1));
			manager.GetStat("mana").AddModifier(new StatModifier(25, StatModifierType.FLAT, -1));

			Assert.That(manager.GetStat("mana").Value, Is.EqualTo(45).Within(assertTolerance));
			Assert.That(observer.Value, Is.EqualTo(45).Within(assertTolerance));
		}

		/// <summary>
		/// Ensure modifiers can be removed and OnValueChanged events are invoked.
		/// </summary>
		[Test]
		public void TestRemoveModifier()
		{
			var manager = new StatManager();

			var observer = new MockStatObserver();

			observer.Observe(manager, "mana");

			manager.AddStat("mana", new Stat(25, 0, 100, true));

			manager.GetStat("mana").AddModifier(new StatModifier(-5, StatModifierType.FLAT, -1));

			var buff = new StatModifier(25, StatModifierType.FLAT, -1);

			manager.GetStat("mana").AddModifier(buff);

			Assert.That(manager.GetStat("mana").Value, Is.EqualTo(45).Within(assertTolerance));

			manager.GetStat("mana").RemoveModifier(buff);

			Assert.That(manager.GetStat("mana").Value, Is.EqualTo(20).Within(assertTolerance));
			Assert.That(observer.Value, Is.EqualTo(20).Within(assertTolerance));
		}

		/// <summary>
		/// Ensure all modifiers from a given source are removed and an OnValueChanged event is
		/// invoked.
		/// </summary>
		[Test]
		public void TestRemoveModifiersFromSource()
		{
			var manager = new StatManager();

			var observer = new MockStatObserver();

			observer.Observe(manager, "mana");

			manager.AddStat("mana", new Stat(25, 0, 100, true));

			var sourceA = new MockStatModifierSource();
			var sourceB = new MockStatModifierSource();

			manager.GetStat("mana").AddModifier(
				new StatModifier(-5, StatModifierType.FLAT, -1, sourceA));
			manager.GetStat("mana").AddModifier(
				new StatModifier(-10, StatModifierType.FLAT, -1, sourceB));
			manager.GetStat("mana").AddModifier(
				new StatModifier(25, StatModifierType.FLAT, -1, sourceA));
			manager.GetStat("mana").AddModifier(
				new StatModifier(15, StatModifierType.FLAT, -1));
			manager.GetStat("mana").AddModifier(
				new StatModifier(10, StatModifierType.FLAT, -1, sourceB));

			Assert.That(manager.GetStat("mana").Value, Is.EqualTo(60).Within(assertTolerance));
			Assert.That(observer.Value, Is.EqualTo(60).Within(assertTolerance));

			manager.GetStat("mana").RemoveModifiersFromSource(sourceA);

			Assert.That(manager.GetStat("mana").Value, Is.EqualTo(40).Within(assertTolerance));
			Assert.That(observer.Value, Is.EqualTo(40).Within(assertTolerance));
		}

		/// <summary>
		/// Ensure OnValueChanged events are propagated from from individual stats through the
		/// manager.
		/// </summary>
		[Test]
		public void TestOnValueChanged()
		{
			var manager = new StatManager();

			var observer = new MockStatObserver();

			observer.Observe(manager, "mana");

			manager.AddStat("mana", new Stat(25, 0, 100, true));

			manager.GetStat("mana").BaseValue = 81;

			Assert.That(manager.GetStat("mana").Value, Is.EqualTo(81).Within(assertTolerance));
			Assert.That(observer.Value, Is.EqualTo(81).Within(assertTolerance));
		}
	}
}
