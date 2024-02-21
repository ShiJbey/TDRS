namespace TDRS
{
	/// <summary>
	/// Constructs RemoveAgentTrait effect instances
	/// </summary>
	public class RemoveAgentTraitFactory : IEffectFactory
	{
		public string EffectName => "RemoveAgentTrait";

		public IEffect CreateInstance(EffectContext ctx, params string[] args)
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

			if (!ctx.Engine.HasAgent(ctx.Bindings[agentVar].ToString()))
			{
				throw new System.ArgumentException(
					$"No Agent found with UID: {ctx.Bindings[agentVar]}"
				);
			}

			return new RemoveAgentTrait(
				ctx.Engine.GetAgent(
					ctx.Bindings[agentVar].ToString()
				),
				traitID
			);
		}
	}
}
