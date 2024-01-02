using YamlDotNet.RepresentationModel;
using TDRS.Helpers;

namespace TDRS
{
	public class StatBuffEffectFactory : IEffectFactory
	{
		public IEffect Instantiate(SocialEngine engine, YamlNode preconditionNode)
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
