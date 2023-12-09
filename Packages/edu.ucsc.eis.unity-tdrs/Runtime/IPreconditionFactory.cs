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
		/// <param name="manager"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public IPrecondition Instantiate(TDRSManager manager, YamlNode preconditionNode);
	}
}
