using System.Collections.Generic;
using NUnit.Framework;

namespace TDRS.Tests
{
	public class TestSocialEngine
	{
		private SocialEngine _state;

		[SetUp]
		public void SetUp()
		{
			_state = SocialEngine.CreateState();

			_state.AgentConfigs["agent"] = new AgentConfig()
			{
				agentType = "agent",
				stats = new StatSchema[0],
				traits = new string[0]
			};

			_state.RelationshipConfigs[("agent", "agent")] = new RelationshipConfig()
			{
				ownerAgentType = "agent",
				targetAgentType = "agent",
				stats = new StatSchema[0],
				traits = new string[0]
			};
		}

		[Test]
		public void TestAddAgent()
		{
			Agent agent = _state.AddAgent("agent", "jose");

			Assert.NotNull(agent);

			Assert.Throws<KeyNotFoundException>(() =>
			{
				_state.AddAgent("character", "lisa");
			});
		}

		[Test]
		public void TestAddRelationship()
		{
			Agent jose = _state.AddAgent("agent", "jose");
			Agent lisa = _state.AddAgent("agent", "lisa");
			Relationship jose_to_lisa = _state.AddRelationship(jose, lisa);
			Assert.NotNull(jose_to_lisa);
			Relationship lisa_to_jose = _state.AddRelationship(lisa.UID, jose.UID);
			Assert.NotNull(lisa_to_jose);
		}

		[Test]
		public void TestGetAgent()
		{
			_state.AddAgent("agent", "jose");

			Agent jose = _state.GetAgent("jose");

			Assert.Throws<KeyNotFoundException>(() =>
			{
				_state.GetAgent("lisa");
			});
		}

		[Test]
		public void TestGetRelationship()
		{
			Agent jose = _state.AddAgent("agent", "jose");
			Agent lisa = _state.AddAgent("agent", "lisa");

			Assert.Throws<KeyNotFoundException>(() =>
			{
				_state.GetRelationship(jose.UID, lisa.UID);
			});

			_state.AddRelationship(jose, lisa);

			Assert.NotNull(_state.GetRelationship(jose.UID, lisa.UID));
		}

		[Test]
		public void TestTryGetAgent()
		{
			Agent agent;

			_state.TryGetAgent("lisa", out agent);

			Assert.That(agent, Is.Null);

			_state.AddAgent("agent", "lisa");

			_state.TryGetAgent("lisa", out agent);

			Assert.NotNull(agent);
		}

		[Test]
		public void TestTryGetRelationship()
		{
			Relationship relationship;

			_state.TryGetRelationship("jose", "lisa", out relationship);

			Assert.That(relationship, Is.Null);

			Agent jose = _state.AddAgent("agent", "jose");
			Agent lisa = _state.AddAgent("agent", "lisa");
			_state.AddRelationship(jose, lisa);

			_state.TryGetRelationship("jose", "lisa", out relationship);

			Assert.NotNull(relationship);
		}

		[Test]
		public void TestRemoveAgent()
		{
			_state.AddAgent("agent", "jose");
			_state.AddAgent("agent", "lisa");
			_state.AddAgent("agent", "sara");

			_state.AddRelationship("jose", "lisa");
			_state.AddRelationship("jose", "sara");

			_state.AddRelationship("sara", "jose");
			_state.AddRelationship("sara", "lisa");

			_state.AddRelationship("lisa", "jose");
			_state.AddRelationship("lisa", "sara");

			// Removing an agent should remove themselves and their
			// incoming and outgoing relationships

			_state.RemoveAgent("jose");

			Assert.That(_state.HasAgent("jose"), Is.False);
			Assert.That(_state.HasRelationship("jose", "lisa"), Is.False);
			Assert.That(_state.HasRelationship("jose", "sara"), Is.False);
			Assert.That(_state.HasRelationship("lisa", "jose"), Is.False);
			Assert.That(_state.HasRelationship("sara", "jose"), Is.False);
		}

		[Test]
		public void TestRemoveRelationship()
		{
			_state.AddAgent("agent", "jose");
			_state.AddAgent("agent", "lisa");
			_state.AddAgent("agent", "sara");

			_state.AddRelationship("jose", "lisa");
			_state.AddRelationship("jose", "sara");

			_state.AddRelationship("sara", "jose");
			_state.AddRelationship("sara", "lisa");

			_state.AddRelationship("lisa", "jose");
			_state.AddRelationship("lisa", "sara");

			_state.RemoveRelationship("sara", "lisa");

			Assert.That(_state.HasRelationship("sara", "lisa"), Is.False);
			Assert.That(_state.HasRelationship("lisa", "sara"), Is.True);
		}

		/// <summary>
		/// Ensure agents and relationships tick properly.
		/// </summary>
		[Test]
		public void TestTick()
		{
			Assert.IsTrue(false);
		}

		/// <summary>
		/// Ensure that events are dispatched properly.
		/// </summary>
		[Test]
		public void TestDispatchEvent()
		{
			Assert.IsTrue(false);
		}

		/// <summary>
		/// Ensure all agents and relationships are removed from the state.
		/// </summary>
		[Test]
		public void TestReset()
		{
			_state.AddAgent("agent", "jose");
			_state.AddAgent("agent", "lisa");
			_state.AddAgent("agent", "sara");

			_state.AddRelationship("jose", "lisa");
			_state.AddRelationship("jose", "sara");

			_state.AddRelationship("sara", "jose");
			_state.AddRelationship("sara", "lisa");

			_state.AddRelationship("lisa", "jose");
			_state.AddRelationship("lisa", "sara");

			_state.Reset();

			Assert.That(_state.HasAgent("jose"), Is.False);
			Assert.That(_state.HasAgent("lisa"), Is.False);
			Assert.That(_state.HasAgent("sara"), Is.False);
		}
	}
}
