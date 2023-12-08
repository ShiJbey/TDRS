using YamlDotNet.RepresentationModel;

namespace TDRS
{
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
