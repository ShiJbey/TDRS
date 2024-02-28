using YamlDotNet.Serialization;

namespace TDRS.Serialization
{
	public class TdrsJsonImporter
	{
		public SocialEngine Import(string jsonString)
		{
			return Import(SocialEngine.Instantiate(), jsonString);
		}

		public SocialEngine Import(SocialEngine socialEngine, string jsonString)
		{
			var deserializer = new DeserializerBuilder()
								.Build();

			var serializedEngine = deserializer.Deserialize<SerializedSocialEngine>(jsonString);

			return SerializedSocialEngine.Deserialize(socialEngine, serializedEngine);
		}
	}
}
