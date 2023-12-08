#nullable enable

using UnityEngine;
using YamlDotNet.RepresentationModel;
using TDRS.Helpers;
using System.Collections.Generic;

namespace TDRS.Sample
{
	[CreateAssetMenu(menuName = "TDRS/Effects/AddSocialRule")]
	public class AddSocialRuleFactory : EffectFactorySO
	{
		public override IEffect Instantiate(TDRSManager manager, YamlNode effectNode)
		{
			bool isOutgoing = true;

			var isOutgoingNode = effectNode.GetChild("is_outgoing");
			if (isOutgoingNode != null) isOutgoing = bool.Parse(isOutgoingNode.GetValue("true"));
			var effects = new List<IEffect>();
			var preconditions = new List<IPrecondition>();

			return new AddSocialRule(preconditions, effects, isOutgoing);
		}
	}
}
