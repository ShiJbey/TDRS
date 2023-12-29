using YamlDotNet.RepresentationModel;

namespace TDRS
{
	/// <summary>
	/// A factory object that creates precondition instances.
	/// </summary>
	public interface IPreconditionFactory
	{
		/// <summary>
		/// Construct a new instance of a Precondition.
		/// </summary>
		/// <param name="engine"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public IPrecondition Instantiate(SocialEngine engine, YamlNode preconditionNode);
	}
}
