namespace TDRS
{
	/// <summary>
	/// Constructs AddAgentTrait effect instances
	/// </summary>
	public class AddAgentTraitFactory : IEffectFactory
	{
		public string EffectName => "AddAgentTrait";

		public IEffect CreateInstance(
			EffectBindingContext ctx,
			params string[] args
		)
		{
			if (args.Length < 2)
			{
				string argStr = string.Join(" ", args);

				throw new System.ArgumentException(
					$"Incorrect number of arguments for 'AddAgentTrait {argStr}'. "
					+ $"Expected at least 2 but was {args.Length}."
				);
			}

			string agentVar = args[0];
			string traitID = args[1];
			int duration = -1;

			if (!ctx.State.HasAgent(ctx.Bindings[agentVar]))
			{
				throw new System.ArgumentException(
					$"No Agent found with ID: {ctx.Bindings[agentVar]}"
				);
			}

			if (args.Length >= 3)
			{
				if (!int.TryParse(args[2], out var value))
				{
					throw new System.ArgumentException(
						$"Expected integer as 3rd argument but was '{args[2]}'"
					);
				}
				duration = value;
			}


			return new AddAgentTrait(
				ctx.State.GetAgent(ctx.Bindings[agentVar]),
				traitID,
				duration
			);
		}
	}
}
