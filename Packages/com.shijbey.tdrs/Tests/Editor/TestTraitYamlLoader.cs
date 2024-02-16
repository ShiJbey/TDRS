using System.Collections.Generic;
using NUnit.Framework;

namespace TDRS.Tests
{
	/// <summary>
	/// Test the TraitYamlLoader class.
	/// </summary>
	public class TestTraitYamlLoader
	{
		[Test]
		public void TestLoadTrait()
		{
			const string yamlString = @"
traitID: jerk
traitType: agent
displayName: Jerk
description: ""[owner] is a jerk""
modifiers:
- statName: Confidence
  value: 5
conflictingTraits:
- friendly";

			var loader = new TraitYamlLoader();

			Trait trait = loader.LoadTrait(yamlString);

			Assert.That(trait.DisplayName, Is.EqualTo("Jerk"));
			Assert.That(trait.Description, Is.EqualTo("[owner] is a jerk"));
			Assert.That(trait.Modifiers.Length, Is.EqualTo(1));
			Assert.That(trait.ConflictingTraits.Contains("friendly"), Is.True);
		}

		[Test]
		public void TestLoadTraits()
		{
			const string yamlString = @"
- traitID: jerk
  traitType: agent
  displayName: Jerk
  description: ""[owner] is a jerk""
  modifiers:
    - statName: Confidence
      value: 5
  conflictingTraits:
    - friendly

- traitID: friendly
  traitType: agent
  displayName: Friendly
  description: ""[owner] is friendly.""
  modifiers:
    - statName: Sociability
      value: 10";

			var loader = new TraitYamlLoader();

			List<Trait> traits = loader.LoadTraits(yamlString);

			Assert.That(traits.Count, Is.EqualTo(2));

			Assert.That(traits[0].DisplayName, Is.EqualTo("Jerk"));
			Assert.That(traits[0].Description, Is.EqualTo("[owner] is a jerk"));
			Assert.That(traits[0].Modifiers.Length, Is.EqualTo(1));
			Assert.That(traits[0].ConflictingTraits.Contains("friendly"), Is.True);

			Assert.That(traits[1].DisplayName, Is.EqualTo("Friendly"));
			Assert.That(traits[1].Description, Is.EqualTo("[owner] is friendly."));
			Assert.That(traits[1].Modifiers.Length, Is.EqualTo(1));
			Assert.That(traits[1].ConflictingTraits.Count, Is.EqualTo(0));
		}
	}
}
