namespace TDRS
{
	/// <summary>
	/// Constructs DecreaseAgentStat effect instances
	/// </summary>
	public class DecreaseAgentStatFactory : IEffectFactory
	{
		public string EffectName => "DecreaseAgentStat";

		public IEffect CreateInstance(EffectContext ctx, params string[] args)
		{
			if (args.Length < 3)
			{
				string argStr = string.Join(" ", args);

				throw new System.ArgumentException(
					$"Incorrect number of arguments for 'DecreaseAgentStat {argStr}'. "
					+ $"Expected at least 3 but was {args.Length}."
				);
			}

			string agentVar = args[0];
			string statName = args[1];

			if (!ctx.Engine.HasAgent(ctx.Bindings[agentVar].ToString()))
			{
				throw new System.ArgumentException(
					$"No Agent found with ID: {ctx.Bindings[agentVar]}"
				);
			}

			if (!float.TryParse(args[2], out var value))
			{
				throw new System.ArgumentException(
					$"Expected number as last argument but was '{args[2]}'"
				);
			}

			int duration = -1;

			if (args.Length >= 4)
			{
				if (!int.TryParse(args[3], out duration))
				{
					throw new System.ArgumentException(
						$"Expected integer as 4th argument but was '{args[3]}'"
					);
				}
			}

			return new DecreaseAgentStat(
				ctx,
				ctx.Engine.GetAgent(
					ctx.Bindings[agentVar].ToString()
				),
				statName,
				value,
				duration
			);
		}
	}
}