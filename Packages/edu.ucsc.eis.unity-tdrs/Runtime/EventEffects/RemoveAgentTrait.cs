using UnityEngine;

namespace TDRS
{
	public class RemoveAgentTraitFactory : SocialEventEffectFactory
	{
		public class RemoveAgentTrait : ISocialEventEffect
		{
			protected SocialAgent m_agent;
			protected string m_traitID;

			public RemoveAgentTrait(
				SocialAgent agent,
				string traitID
			)
			{
				m_agent = agent;
				m_traitID = traitID;
			}

			public void Apply()
			{
				m_agent.RemoveTrait(m_traitID);
			}
		}

		public override string EffectType => "AddAgentTrait";

		public override ISocialEventEffect CreateInstance(SocialEventContext ctx, params string[] args)
		{
			if (args.Length != 2)
			{
				throw new System.ArgumentException(
					"Incorrect number of arguments for RemoveAgentTrait. "
					+ $"Expected 2 but was {args.Length}."
				);
			}

			string agentVar = args[0];
			string traitID = args[1];

			return new RemoveAgentTrait(
				ctx.Engine.GetAgent(
					ctx.Bindings[agentVar]
				),
				traitID
			);
		}
	}
}
