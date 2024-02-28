using YamlDotNet.Serialization;

namespace TDRS.Serialization
{
	public class TdrsJsonExporter
	{
		public string Export(SocialEngine socialEngine)
		{
			var serializedEngine = SerializedSocialEngine.Serialize(socialEngine);

			var serializer = new SerializerBuilder()
					.JsonCompatible()
					.Build();

			return serializer.Serialize(serializedEngine);
		}
	}
}
