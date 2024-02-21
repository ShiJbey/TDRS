using NUnit.Framework;

namespace TDRS.Tests
{
	public class TestRelationship
	{
		private SocialEngine _engine;

		[SetUp]
		public void SetUp()
		{
			_engine = SocialEngine.Instantiate();

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

			_engine.TraitLibrary.AddTrait(
				new Trait(
					traitID: "dating",
					traitType: TraitType.Relationship,
					displayName: "Dating",
					description: "[owner] and [target] are dating",
					modifiers: new StatModifierData[0],
					conflictingTraits: new string[0]
				)
			);

			_engine.TraitLibrary.AddTrait(
				new Trait(
					traitID: "strangers",
					traitType: TraitType.Relationship,
					displayName: "Strangers",
					description: "[owner] and [target] are strangers",
					modifiers: new StatModifierData[0],
					conflictingTraits: new string[0]
				)
			);
		}

		[Test]
		public void TestRelationshipType()
		{
			_engine.AddAgent("character", "lisa");
			_engine.AddAgent("character", "sara");

			var lisaToSara = _engine.AddRelationship("lisa", "sara");

			Assert.That(lisaToSara.RelationshipType, Is.Null);
			Assert.That(
				_engine.DB.Assert("lisa.relationships.sara.type"),
				Is.False
			);

			lisaToSara.SetRelationshipType("strangers");

			Assert.That(lisaToSara.RelationshipType.TraitID, Is.EqualTo("strangers"));
			Assert.That(
				_engine.DB.Assert("lisa.relationships.sara.type!strangers"),
				Is.True
			);

			lisaToSara.SetRelationshipType("dating");

			Assert.That(lisaToSara.RelationshipType.TraitID, Is.EqualTo("dating"));
			Assert.That(
				_engine.DB.Assert("lisa.relationships.sara.type!dating"),
				Is.True
			);
		}
	}
}
