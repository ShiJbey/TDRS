using YamlDotNet.RepresentationModel;

namespace TDRS
{
	public interface IEffectFactory
	{
		/// <summary>
		/// Construct a new instance of an Effect.
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="effectNode"></param>
		/// <returns></returns>
		public IEffect Instantiate(TDRSManager manager, YamlNode effectNode);
	}
}
