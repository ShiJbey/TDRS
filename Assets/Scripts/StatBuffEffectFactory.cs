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

			string statName = ((YamlScalarNode)mapping.GetChild("stat")).GetValue();

			float amount = float.Parse(
				((YamlScalarNode)mapping.GetChild("amount")).GetValue()
			);

			var reasonNode = mapping.TryGetChild("reason");
			string reason = reasonNode != null ? reasonNode.GetValue() : "";

			return new StatBuffEffect(statName, amount, reason);
		}
	}
}
