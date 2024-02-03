using System.IO;
using NUnit.Framework;
using TDRS;
using TDRS.Helpers;
using YamlDotNet.RepresentationModel;

public class TestJsonExportImport
{
	[Test]
	public void TestExportJson()
	{
		var engineState = SocialEngineState.CreateState();

		var jsonExporter = new JsonExporter();

		var jsonData = jsonExporter.ToJson(engineState);

		// Reload the raw json
		var yaml = new YamlStream();
		yaml.Load(new StringReader(jsonData));

		YamlDocument doc = yaml.Documents[0];

		// Check that the document has a property for the format_version
		var formatVersionNode = doc.RootNode.TryGetChild("formatVersion");
		Assert.NotNull(formatVersionNode);
	}

	[Test]
	public void TestImportJson()
	{
		const string jsonData = @"
		{
			""formatVersion"": 1
		}
		";

		var engineState = SocialEngineState.CreateState();

		var jsonImporter = new JsonImporter();

		jsonImporter.LoadJson(engineState, jsonData);
	}
}
