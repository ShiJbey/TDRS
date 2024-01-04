using YamlDotNet.RepresentationModel;
using TDRS.Helpers;

namespace TDRS
{
	public class TargetHasTraitFactory : PreconditionFactory
	{
		public override IPrecondition Instantiate(SocialEngine engine, YamlNode preconditionNode)
		{
			var mapping = (YamlMappingNode)preconditionNode;

			var traitID = ((YamlScalarNode)mapping.GetChild("trait")).GetValue();

			return new TargetHasTrait(traitID);
		}
	}
}
