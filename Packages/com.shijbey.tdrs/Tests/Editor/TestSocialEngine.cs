using System.Collections.Generic;
using NUnit.Framework;

namespace TDRS.Tests
{
	public class TestSocialEngine
	{
		private SocialEngine _engine;

		[SetUp]
		public void SetUp()
		{
			_engine = SocialEngine.Instantiate();

			_engine.AddAgentSchema(
				new AgentSchema(
					"agent",
					new StatSchema[0],
					new string[0]
				)
			);

			_engine.AddRelationshipSchema(
				new RelationshipSchema(
					"agent",
					"agent",
					new StatSchema[0],
					new string[0]
				)
			);
		}

		[Test]
		public void TestAddAgent()
		{
			Agent agent = _engine.AddAgent("agent", "jose");

			Assert.NotNull(agent);

			Assert.Throws<KeyNotFoundException>(() =>
			{
				_engine.AddAgent("character", "lisa");
			});
		}

		[Test]
		public void TestAddRelationship()
		{
			Agent jose = _engine.AddAgent("agent", "jose");
			Agent lisa = _engine.AddAgent("agent", "lisa");
			Relationship jose_to_lisa = _engine.AddRelationship(jose, lisa);
			Assert.NotNull(jose_to_lisa);
			Relationship lisa_to_jose = _engine.AddRelationship(lisa.UID, jose.UID);
			Assert.NotNull(lisa_to_jose);
		}

		[Test]
		public void TestGetAgent()
		{
			_engine.AddAgent("agent", "jose");

			Agent jose = _engine.GetAgent("jose");

			Assert.Throws<KeyNotFoundException>(() =>
			{
				_engine.GetAgent("lisa");
			});
		}

		[Test]
		public void TestGetRelationship()
		{
			Agent jose = _engine.AddAgent("agent", "jose");
			Agent lisa = _engine.AddAgent("agent", "lisa");

			Assert.Throws<KeyNotFoundException>(() =>
			{
				_engine.GetRelationship(jose.UID, lisa.UID);
			});

			_engine.AddRelationship(jose, lisa);

			Assert.NotNull(_engine.GetRelationship(jose.UID, lisa.UID));
		}

		[Test]
		public void TestTryGetAgent()
		{
			Agent agent;

			_engine.TryGetAgent("lisa", out agent);

			Assert.That(agent, Is.Null);

			_engine.AddAgent("agent", "lisa");

			_engine.TryGetAgent("lisa", out agent);

			Assert.NotNull(agent);
		}

		[Test]
		public void TestTryGetRelationship()
		{
			Relationship relationship;

			_engine.TryGetRelationship("jose", "lisa", out relationship);

			Assert.That(relationship, Is.Null);

			Agent jose = _engine.AddAgent("agent", "jose");
			Agent lisa = _engine.AddAgent("agent", "lisa");
			_engine.AddRelationship(jose, lisa);

			_engine.TryGetRelationship("jose", "lisa", out relationship);

			Assert.NotNull(relationship);
		}

		[Test]
		public void TestRemoveAgent()
		{
			_engine.AddAgent("agent", "jose");
			_engine.AddAgent("agent", "lisa");
			_engine.AddAgent("agent", "sara");

			_engine.AddRelationship("jose", "lisa");
			_engine.AddRelationship("jose", "sara");

			_engine.AddRelationship("sara", "jose");
			_engine.AddRelationship("sara", "lisa");

			_engine.AddRelationship("lisa", "jose");
			_engine.AddRelationship("lisa", "sara");

			// Removing an agent should remove themselves and their
			// incoming and outgoing relationships

			_engine.RemoveAgent("jose");

			Assert.That(_engine.HasAgent("jose"), Is.False);
			Assert.That(_engine.HasRelationship("jose", "lisa"), Is.False);
			Assert.That(_engine.HasRelationship("jose", "sara"), Is.False);
			Assert.That(_engine.HasRelationship("lisa", "jose"), Is.False);
			Assert.That(_engine.HasRelationship("sara", "jose"), Is.False);
		}

		[Test]
		public void TestRemoveRelationship()
		{
			_engine.AddAgent("agent", "jose");
			_engine.AddAgent("agent", "lisa");
			_engine.AddAgent("agent", "sara");

			_engine.AddRelationship("jose", "lisa");
			_engine.AddRelationship("jose", "sara");

			_engine.AddRelationship("sara", "jose");
			_engine.AddRelationship("sara", "lisa");

			_engine.AddRelationship("lisa", "jose");
			_engine.AddRelationship("lisa", "sara");

			_engine.RemoveRelationship("sara", "lisa");

			Assert.That(_engine.HasRelationship("sara", "lisa"), Is.False);
			Assert.That(_engine.HasRelationship("lisa", "sara"), Is.True);
		}

		/// <summary>
		/// Ensure all agents and relationships are removed from the state.
		/// </summary>
		[Test]
		public void TestReset()
		{
			_engine.AddAgent("agent", "jose");
			_engine.AddAgent("agent", "lisa");
			_engine.AddAgent("agent", "sara");

			_engine.AddRelationship("jose", "lisa");
			_engine.AddRelationship("jose", "sara");

			_engine.AddRelationship("sara", "jose");
			_engine.AddRelationship("sara", "lisa");

			_engine.AddRelationship("lisa", "jose");
			_engine.AddRelationship("lisa", "sara");

			_engine.Reset();

			Assert.That(_engine.HasAgent("jose"), Is.False);
			Assert.That(_engine.HasAgent("lisa"), Is.False);
			Assert.That(_engine.HasAgent("sara"), Is.False);
		}
	}
}
