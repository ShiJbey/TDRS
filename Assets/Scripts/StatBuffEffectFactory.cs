#nullable enable

using UnityEngine;
using YamlDotNet.RepresentationModel;
using TDRS.Helpers;

namespace TDRS.Sample
{
	[CreateAssetMenu(menuName = "TDRS/Effects/StatBuff")]
	public class StatBuffEffectFactory : EffectFactorySO
	{
		public override IEffect Instantiate(TDRSManager manager, YamlNode preconditionNode)
		{
			var mapping = (YamlMappingNode)preconditionNode;

			var statName = ((YamlScalarNode)mapping.GetChild("stat")).GetValue();

			var amount = float.Parse(
				((YamlScalarNode)mapping.GetChild("amount")).GetValue()
			);

			return new StatBuffEffect(statName, amount);
		}
	}
}
