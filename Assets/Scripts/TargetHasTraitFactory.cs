#nullable enable

using UnityEngine;
using YamlDotNet.RepresentationModel;
using TDRS.Helpers;

namespace TDRS.Sample
{
	[CreateAssetMenu(menuName = "TDRS/Preconditions/TargetHasTrait")]
	public class TargetHasTraitFactory : PreconditionFactorySO
	{
		public override IPrecondition Instantiate(TDRSManager manager, YamlNode preconditionNode)
		{
			var mapping = (YamlMappingNode)preconditionNode;

			var traitID = ((YamlScalarNode)mapping.GetChild("trait")).GetValue();

			return new TargetHasTrait(traitID);
		}
	}
}
