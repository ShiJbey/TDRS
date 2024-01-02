using YamlDotNet.RepresentationModel;

namespace TDRS
{
	/// <summary>
	/// An object that creates instances of effects.
	/// </summary>
	public interface IEffectFactory
	{
		/// <summary>
		/// Construct a new instance of an Effect.
		/// </summary>
		/// <param name="engine"></param>
		/// <param name="effectNode"></param>
		/// <returns></returns>
		public IEffect Instantiate(SocialEngine engine, YamlNode effectNode);
	}
}
