using YamlDotNet.RepresentationModel;
using TDRS.Helpers;
using System.Collections.Generic;

namespace TDRS
{
	public class AddSocialRuleFactory : IEffectFactory
	{
		public IEffect Instantiate(SocialEngine engine, YamlNode effectNode)
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
					var factory = engine.PreconditionLibrary.GetPreconditionFactory(preconditionType);
					IPrecondition precondition = factory.Instantiate(engine, entry);
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
					var factory = engine.EffectLibrary.GetEffectFactory(effectType);
					var effect = factory.Instantiate(engine, entry);
					effects.Add(effect);
				}
			}

			return new AddSocialRule(preconditions, effects, isOutgoing);
		}
	}
}
