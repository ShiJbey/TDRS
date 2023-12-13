using YamlDotNet.RepresentationModel;
using TDRS.Helpers;

namespace TDRS
{
	public class OwnerHasTraitFactory : IPreconditionFactory
	{
		public IPrecondition Instantiate(TDRSManager manager, YamlNode preconditionNode)
		{
			var mapping = (YamlMappingNode)preconditionNode;

			var traitID = ((YamlScalarNode)mapping.GetChild("trait")).GetValue();

			return new OwnerHasTrait(traitID);
		}
	}
}
