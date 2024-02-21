using System;
using System.Xml.Serialization;
using NUnit.Framework;

namespace TDRS.Tests
{
	/// <summary>
	/// Tests the TDRS.Stat class and associated methods.
	/// </summary>
	public class TestStat
	{
		/// <summary>
		/// Tolerance threshold for floating point equality assertions.
		/// </summary>
		private double assertTolerance;

		[SetUp]
		public void SetUp()
		{
			assertTolerance = Math.Pow(10, -Stat.ROUND_PRECISION);
		}

		/// <summary>
		/// Plays the role of an example object that might subscribe to a stat.
		/// </summary>
		private class MockStatObserver
		{
			public float Value { get; private set; }

			public void ObserveStat(Stat stat)
			{
				Value = stat.Value;
				stat.OnValueChanged += HandleUpdate;
			}

			public void StopObserveStat(Stat stat)
			{
				stat.OnValueChanged -= HandleUpdate;
			}

			private void HandleUpdate(object obj, Stat.OnValueChangedArgs args)
			{
				Value = args.Value;
			}
		}

		/// <summary>
		/// A placeholder source for stat used in unity tests.
		/// </summary>
		private class MockStatModifierSource { }

		/// <summary>
		/// Ensure setting IsDiscrete floors stat values to integer equivalents.
		/// </summary>
		[Test]
		public void TestDiscreteValue()
		{
			// Continuously-valued strength stat
			Stat strengthCont = new Stat(45.5f, 0f, 100f, false);

			Assert.That(strengthCont.Value, Is.EqualTo(45.5f));

			// Discretely-valued strength stat
			Stat strengthDisc = new Stat(45.5f, 0f, 100f, true);

			Assert.That(strengthDisc.Value, Is.EqualTo(45f));
		}

		/// <summary>
		/// Ensure the value of a stat is always clamped by the min and max values.
		/// </summary>
		[Test]
		public void TestValueClamped()
		{
			Stat insecurity = new Stat(123, 0, 50, true);

			Assert.That(insecurity.Value, Is.EqualTo(50));

			Stat malice = new Stat(-54, -50, 50, true);

			Assert.That(malice.Value, Is.EqualTo(-50));
		}

		/// <summary>
		/// Ensure changing the BaseValue property updates the Value and triggers an
		/// OnValueChange event.
		/// </summary>
		[Test]
		public void TestBaseValueChangesValue()
		{
			Stat compassion = new Stat(0, 0, 100, true);

			Assert.That(compassion.Value, Is.EqualTo(0));

			compassion.BaseValue = 30;

			Assert.That(compassion.Value, Is.EqualTo(30));

			var observer = new MockStatObserver();

			observer.ObserveStat(compassion);

			Assert.That(observer.Value, Is.EqualTo(30));

			compassion.BaseValue = 63;

			Assert.That(observer.Value, Is.EqualTo(63));
		}

		/// <summary>
		/// Ensure the Normalized value properly normalizes a stat on the interval from
		/// 0.0 to 1.0
		/// </summary>
		[Test]
		public void TestNormalizedValue()
		{
			Stat compassion = new Stat(0, 0, 100, true);

			Assert.That(compassion.Normalized, Is.EqualTo(0).Within(assertTolerance));

			compassion.BaseValue = 30;

			Assert.That(compassion.Normalized, Is.EqualTo(0.3).Within(assertTolerance));

			compassion.BaseValue = 63;

			Assert.That(compassion.Normalized, Is.EqualTo(0.63).Within(assertTolerance));

			Stat malice = new Stat(-54, -50, 50, true);

			Assert.That(malice.Normalized, Is.EqualTo(0).Within(assertTolerance));

			malice.BaseValue = 0;

			Assert.That(malice.Normalized, Is.EqualTo(0.5).Within(assertTolerance));
		}

		/// <summary>
		/// Ensure adding FLAT modifiers correctly changes the stat value.
		/// </summary>
		[Test]
		public void TestAddModifierFlat()
		{
			var mana = new Stat(25, 0, 100, false);

			var observer = new MockStatObserver();

			observer.ObserveStat(mana);

			var debuff = new StatModifier(-15, StatModifierType.FLAT, -1);

			mana.AddModifier(debuff);

			Assert.That(mana.Value, Is.EqualTo(10).Within(assertTolerance));

			var buff = new StatModifier(50, StatModifierType.FLAT, -1);

			mana.AddModifier(buff);

			Assert.That(mana.Value, Is.EqualTo(60).Within(assertTolerance));

			Assert.That(observer.Value, Is.EqualTo(60).Within(assertTolerance));
		}

		/// <summary>
		/// Ensure adding a PERCENT_ADD modifier correctly changes the stat value.
		/// </summary>
		[Test]
		public void TestAddModifierPercentAdd()
		{
			var mana = new Stat(25, 0, 100, false);

			var observer = new MockStatObserver();

			observer.ObserveStat(mana);

			var debuff = new StatModifier(-0.15f, StatModifierType.PERCENT_ADD, -1);

			mana.AddModifier(debuff);

			Assert.That(mana.Value, Is.EqualTo(21.25).Within(assertTolerance));

			var buff = new StatModifier(0.50f, StatModifierType.PERCENT_ADD, -1);

			mana.AddModifier(buff);

			Assert.That(mana.Value, Is.EqualTo(33.75).Within(assertTolerance));

			Assert.That(observer.Value, Is.EqualTo(33.75).Within(assertTolerance));
		}

		/// <summary>
		/// Ensure adding a PERCENT_MULTIPLY modifier correctly changes the stat value.
		/// </summary>
		[Test]
		public void TestAddModifierPercentMultiply()
		{
			var mana = new Stat(25, 0, 100, false);

			var observer = new MockStatObserver();

			observer.ObserveStat(mana);

			var debuff = new StatModifier(-0.15f, StatModifierType.PERCENT_MULTIPLY, -1);

			mana.AddModifier(debuff);

			Assert.That(mana.Value, Is.EqualTo(21.25).Within(assertTolerance));

			var buff = new StatModifier(0.50f, StatModifierType.PERCENT_MULTIPLY, -1);

			mana.AddModifier(buff);

			Assert.That(mana.Value, Is.EqualTo(31.875).Within(assertTolerance));

			Assert.That(observer.Value, Is.EqualTo(31.875).Within(assertTolerance));
		}

		/// <summary>
		/// Ensure removing FLAT modifiers correctly changes the stat value.
		/// </summary>
		[Test]
		public void TestRemoveModifierFlat()
		{
			var mana = new Stat(25, 0, 100, false);

			var observer = new MockStatObserver();

			observer.ObserveStat(mana);

			var debuff = new StatModifier(-15, StatModifierType.FLAT, -1);

			mana.AddModifier(debuff);

			var buff = new StatModifier(50, StatModifierType.FLAT, -1);

			mana.AddModifier(buff);

			Assert.That(mana.Value, Is.EqualTo(60).Within(assertTolerance));

			mana.RemoveModifier(debuff);

			Assert.That(mana.Value, Is.EqualTo(75).Within(assertTolerance));

			Assert.That(observer.Value, Is.EqualTo(75).Within(assertTolerance));
		}

		/// <summary>
		/// Ensure removing a PERCENT_ADD modifier correctly changes the stat value.
		/// </summary>
		[Test]
		public void TestRemoveModifierPercentAdd()
		{
			var mana = new Stat(25, 0, 100, false);

			var observer = new MockStatObserver();

			observer.ObserveStat(mana);

			var debuff = new StatModifier(-0.15f, StatModifierType.PERCENT_ADD, -1);

			mana.AddModifier(debuff);

			var buff = new StatModifier(0.50f, StatModifierType.PERCENT_ADD, -1);

			mana.AddModifier(buff);

			Assert.That(mana.Value, Is.EqualTo(33.75).Within(assertTolerance));

			mana.RemoveModifier(debuff);

			Assert.That(mana.Value, Is.EqualTo(37.5).Within(assertTolerance));

			Assert.That(observer.Value, Is.EqualTo(37.5).Within(assertTolerance));
		}

		/// <summary>
		/// Ensure remove a PERCENT_MULTIPLY modifier correctly changes the stat value.
		/// </summary>
		[Test]
		public void TestRemoveModifierPercentMultiply()
		{
			var mana = new Stat(25, 0, 100, false);

			var observer = new MockStatObserver();

			observer.ObserveStat(mana);

			var debuff = new StatModifier(-0.15f, StatModifierType.PERCENT_MULTIPLY, -1);

			mana.AddModifier(debuff);

			var buff = new StatModifier(0.50f, StatModifierType.PERCENT_MULTIPLY, -1);

			mana.AddModifier(buff);

			Assert.That(mana.Value, Is.EqualTo(31.875).Within(assertTolerance));

			mana.RemoveModifier(debuff);

			Assert.That(mana.Value, Is.EqualTo(37.5).Within(assertTolerance));

			Assert.That(observer.Value, Is.EqualTo(37.5).Within(assertTolerance));
		}

		/// <summary>
		/// Ensure all modifiers from a given source are removed from a stat.
		/// </summary>
		[Test]
		public void TestRemoveModifiersFromSource()
		{
			var mana = new Stat(25, 0, 100, false);

			var observer = new MockStatObserver();

			observer.ObserveStat(mana);

			var sourceA = new MockStatModifierSource();
			var sourceB = new MockStatModifierSource();

			mana.AddModifier(new StatModifier(-5, StatModifierType.FLAT, -1, sourceA));
			mana.AddModifier(new StatModifier(-10, StatModifierType.FLAT, -1, sourceB));
			mana.AddModifier(new StatModifier(25, StatModifierType.FLAT, -1, sourceA));
			mana.AddModifier(new StatModifier(15, StatModifierType.FLAT, -1));
			mana.AddModifier(new StatModifier(10, StatModifierType.FLAT, -1, sourceB));

			Assert.That(mana.Value, Is.EqualTo(60));

			mana.RemoveModifiersFromSource(sourceB);

			Assert.That(mana.Value, Is.EqualTo(60));

			mana.RemoveModifiersFromSource(sourceA);

			Assert.That(mana.Value, Is.EqualTo(40));

			Assert.That(observer.Value, Is.EqualTo(40).Within(assertTolerance));
		}

		/// <summary>
		/// Ensure that RemoveModifier() returns proper boolean value on success or failure
		/// </summary>
		[Test]
		public void TestRemoveModifierReturnValue()
		{
			var mana = new Stat(25, 0, 100, false);

			var debuff = new StatModifier(-0.15f, StatModifierType.PERCENT_MULTIPLY, -1);
			var buff = new StatModifier(0.50f, StatModifierType.PERCENT_MULTIPLY, -1);

			mana.AddModifier(debuff);

			bool success;

			success = mana.RemoveModifier(debuff);

			Assert.That(success, Is.True);

			success = mana.RemoveModifier(buff);

			Assert.That(success, Is.False);
		}

		/// <summary>
		/// Ensure the proper success state is returned when removing all modifiers from a source.
		/// </summary>
		[Test]
		public void TestRemoveModifiersFromSourceReturnValue()
		{
			var mana = new Stat(25, 0, 100, false);

			var observer = new MockStatObserver();

			observer.ObserveStat(mana);

			var sourceA = new MockStatModifierSource();
			var sourceB = new MockStatModifierSource();
			var sourceC = new MockStatModifierSource();

			mana.AddModifier(new StatModifier(-5, StatModifierType.FLAT, -1, sourceA));
			mana.AddModifier(new StatModifier(-10, StatModifierType.FLAT, -1, sourceB));
			mana.AddModifier(new StatModifier(25, StatModifierType.FLAT, -1, sourceA));
			mana.AddModifier(new StatModifier(15, StatModifierType.FLAT, -1));
			mana.AddModifier(new StatModifier(10, StatModifierType.FLAT, -1, sourceB));

			bool success;

			success = mana.RemoveModifiersFromSource(sourceA);

			Assert.That(success, Is.True);

			success = mana.RemoveModifiersFromSource(sourceC);

			Assert.That(success, Is.False);
		}
	}
}
