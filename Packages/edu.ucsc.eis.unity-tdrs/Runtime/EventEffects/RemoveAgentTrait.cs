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

			public void Remove()
			{
				// Trait removal is permanent
				return;
			}
		}

		public override string EffectName => "AddAgentTrait";

		public override ISocialEventEffect CreateInstance(EffectBindingContext ctx, params string[] args)
		{
			if (args.Length != 2)
			{
				string argStr = string.Join(" ", args);

				throw new System.ArgumentException(
					$"Incorrect number of arguments for 'RemoveAgentTrait {argStr}'. "
					+ $"Expected 2 but was {args.Length}."
				);
			}

			string agentVar = args[0];
			string traitID = args[1];

			if (!ctx.Engine.HasAgent(ctx.Bindings[agentVar]))
			{
				throw new System.ArgumentException(
					$"No Agent found with ID: {ctx.Bindings[agentVar]}"
				);
			}

			return new RemoveAgentTrait(
				ctx.Engine.GetAgent(
					ctx.Bindings[agentVar]
				),
				traitID
			);
		}
	}
}
