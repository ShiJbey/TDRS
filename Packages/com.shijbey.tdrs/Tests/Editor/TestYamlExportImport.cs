using System.IO;
using NUnit.Framework;
using YamlDotNet.RepresentationModel;
using TDRS.Helpers;

namespace TDRS.Tests
{
	public class TestYamlExportImport
	{
		[Test]
		public void TestExportYaml()
		{
			var engine = SocialEngine.CreateState();

			var jsonExporter = new YamlExporter();

			var jsonData = jsonExporter.ToYaml(engine);

			// Reload the raw json
			var yaml = new YamlStream();
			yaml.Load(new StringReader(jsonData));

			YamlDocument doc = yaml.Documents[0];

			// Check that the document has a property for the format_version
			var formatVersionNode = doc.RootNode.TryGetChild("formatVersion");
			Assert.NotNull(formatVersionNode);
		}

		[Test]
		public void TestImportYaml()
		{
			const string jsonData = @"
			{
				""formatVersion"": 1
			}
			";

			var engine = SocialEngine.CreateState();

			var jsonImporter = new YamlImporter();

			jsonImporter.LoadYaml(engine, jsonData);
		}
	}

}
