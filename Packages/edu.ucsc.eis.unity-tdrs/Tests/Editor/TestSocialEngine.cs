using System.Collections.Generic;
using NUnit.Framework;
using TDRS;

public class TestSocialEngine
{
	[Test]
	public void TestCreateNode()
	{
		// var engine = new SocialEngine();

		// engine.AddNodeSchema(new NodeSchema(
		// 	"character",
		// 	new StatSchema[]{
		// 		new StatSchema("sociability", 10, 50, 0, true)
		// 	}
		// ));

		// var ein = engine.CreateNode("character", "ein");

		// Assert.AreEqual("character", ein.NodeType);
		// Assert.IsTrue(ein.Stats.HasStat("sociability"));
		// Assert.AreEqual(10f, ein.Stats.GetStat("sociability").Value);
	}

	[Test]
	public void TestCreateRelationship()
	{
		// var engine = new SocialEngine();

		// engine.AddNodeSchema(new NodeSchema(
		// 	"character",
		// 	new StatSchema[]{
		// 		new StatSchema("sociability", 10, 50, 0, true)
		// 	}
		// ));

		// engine.AddNodeSchema(new NodeSchema(
		// 	"organization",
		// 	new StatSchema[]{
		// 		new StatSchema("influence", 0, 50, 0, true)
		// 	}
		// ));

		// engine.AddRelationshipSchema(new RelationshipSchema(
		// 	"character",
		// 	"character",
		// 	new StatSchema[]{
		// 		new StatSchema("friendship", 0, 50, 0, true)
		// 	}
		// ));

		// engine.AddRelationshipSchema(new RelationshipSchema(
		// 	"character",
		// 	"organization",
		// 	new StatSchema[]{
		// 		new StatSchema("reputation", 0, 50, 0, true)
		// 	}
		// ));

		// engine.AddRelationshipSchema(new RelationshipSchema(
		// 	"organization",
		// 	"character",
		// 	new StatSchema[]{
		// 		new StatSchema("reputation", 0, 50, 0, true)
		// 	}
		// ));

		// engine.CreateNode("character", "ein");
		// engine.CreateNode("character", "zwei");
		// var the_org = engine.CreateNode("organization", "the_org");

		// engine.CreateRelationship("ein", "zwei");
		// var ein_to_org = engine.CreateRelationship("ein", "the_org");
		// engine.CreateRelationship("the_org", "ein");

		// Assert.AreEqual("organization", the_org.NodeType);
		// Assert.IsTrue(ein_to_org.Stats.HasStat("reputation"));
		// Assert.AreEqual(0f, ein_to_org.Stats.GetStat("reputation").Value);
	}

	[Test]
	public void TestGetNode()
	{
		// var engine = new SocialEngine();

		// engine.AddNodeSchema(new NodeSchema(
		// 	"character",
		// 	new StatSchema[]{
		// 		new StatSchema("sociability", 10, 50, 0, true)
		// 	}
		// ));

		// Assert.Throws<KeyNotFoundException>(() => engine.GetNode("ein"));

		// engine.CreateNode("character", "ein");

		// Assert.DoesNotThrow(() => engine.GetNode("ein"));

		// var node = engine.GetNode("ein");

		// Assert.AreEqual("ein", node.UID);
	}

	[Test]
	public void TestGetRelationship()
	{
		// var engine = new SocialEngine();

		// engine.AddNodeSchema(new NodeSchema(
		// 	"character",
		// 	new StatSchema[]{
		// 		new StatSchema("sociability", 10, 50, 0, true)
		// 	}
		// ));

		// engine.AddRelationshipSchema(new RelationshipSchema(
		// 	"character",
		// 	"character",
		// 	new StatSchema[]{
		// 				new StatSchema("friendship", 0, 50, 0, true)
		// 	}
		// ));

		// Assert.Throws<KeyNotFoundException>(() => engine.GetRelationship("ein", "zwei"));

		// engine.CreateNode("character", "ein");
		// engine.CreateNode("character", "zwei");

		// Assert.Throws<KeyNotFoundException>(() => engine.GetRelationship("ein", "zwei"));

		// engine.CreateRelationship("ein", "zwei");

		// Assert.DoesNotThrow(() => engine.GetRelationship("ein", "zwei"));

		// var relationship = engine.GetRelationship("ein", "zwei");

		// Assert.AreEqual("ein", relationship.Owner.UID);
		// Assert.AreEqual("zwei", relationship.Target.UID);
	}

	[Test]
	public void TestHasNode()
	{
		// var engine = new SocialEngine();

		// engine.AddNodeSchema(new NodeSchema(
		// 	"character",
		// 	new StatSchema[]{
		// 		new StatSchema("sociability", 10, 50, 0, true)
		// 	}
		// ));

		// Assert.IsFalse(engine.HasNode("ein"));

		// engine.CreateNode("character", "ein");

		// Assert.IsTrue(engine.HasNode("ein"));
	}

	[Test]
	public void TestHasRelationship()
	{
		// var engine = new SocialEngine();

		// engine.AddNodeSchema(new NodeSchema(
		// 	"character",
		// 	new StatSchema[]{
		// 		new StatSchema("sociability", 10, 50, 0, true)
		// 	}
		// ));

		// engine.AddRelationshipSchema(new RelationshipSchema(
		// 	"character",
		// 	"character",
		// 	new StatSchema[]{
		// 				new StatSchema("friendship", 0, 50, 0, true)
		// 	}
		// ));

		// Assert.IsFalse(engine.HasRelationship("ein", "zwei"));

		// engine.CreateNode("character", "ein");
		// engine.CreateNode("character", "zwei");
		// engine.CreateRelationship("ein", "zwei");

		// Assert.IsTrue(engine.HasRelationship("ein", "zwei"));
	}

	[Test]
	public void TestTryGetNode()
	{
		// var engine = new SocialEngine();

		// engine.AddNodeSchema(new NodeSchema(
		// 	"character",
		// 	new StatSchema[]{
		// 		new StatSchema("sociability", 10, 50, 0, true)
		// 	}
		// ));

		// TDRSNode node;
		// bool success;

		// success = engine.TryGetNode("ein", out node);

		// Assert.IsFalse(success);
		// Assert.IsNull(node);

		// engine.CreateNode("character", "ein");

		// success = engine.TryGetNode("ein", out node);

		// Assert.IsTrue(success);
		// Assert.IsNotNull(node);
	}

	[Test]
	public void TestTryGetRelationship()
	{
		// var engine = new SocialEngine();

		// engine.AddNodeSchema(new NodeSchema(
		// 	"character",
		// 	new StatSchema[]{
		// 		new StatSchema("sociability", 10, 50, 0, true)
		// 	}
		// ));

		// engine.AddRelationshipSchema(new RelationshipSchema(
		// 	"character",
		// 	"character",
		// 	new StatSchema[]{
		// 				new StatSchema("friendship", 0, 50, 0, true)
		// 	}
		// ));

		// TDRSRelationship relationship;
		// bool success;

		// success = engine.TryGetRelationship("ein", "zwei", out relationship);

		// Assert.IsFalse(success);
		// Assert.IsNull(relationship);

		// engine.CreateNode("character", "ein");
		// engine.CreateNode("character", "zwei");
		// engine.CreateRelationship("ein", "zwei");

		// success = engine.TryGetRelationship("ein", "zwei", out relationship);

		// Assert.IsTrue(success);
		// Assert.IsNotNull(relationship);
	}
}
