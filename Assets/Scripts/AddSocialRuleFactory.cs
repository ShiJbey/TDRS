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

			List<IEffect> effects = new List<IEffect>();
			List<IPrecondition> preconditions = new List<IPrecondition>();

			// Need to parse preconditions
			YamlNode preconditionsNode = effectNode.TryGetChild("preconditions");
			if (preconditionsNode != null)
			{
				var sequence = (YamlSequenceNode)preconditionsNode;

				foreach (var entry in sequence)
				{
					string preconditionType = entry.GetChild("type").GetValue();
					var factory = manager.PreconditionLibrary.GetPreconditionFactory(preconditionType);
					IPrecondition precondition = factory.Instantiate(manager, entry);
					preconditions.Add(precondition);
				}
			}

			// Need to parse effects
			YamlNode effectsNode = effectNode.TryGetChild("effects");
			if (effectsNode != null)
			{
				var sequence = (YamlSequenceNode)effectsNode;

				foreach (var entry in sequence)
				{
					var effectType = entry.GetChild("type").GetValue();
					var factory = manager.EffectLibrary.GetEffectFactory(effectType);
					var effect = factory.Instantiate(manager, entry);
					effects.Add(effect);
				}
			}

			return new AddSocialRule(preconditions, effects, isOutgoing);
		}
	}
}
