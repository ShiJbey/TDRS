namespace TDRS
{
	public class AddAgentTrait : Effect
	{
		#region Fields

		private Agent m_agent;
		private string m_traitID;

		#endregion

		#region Properties

		public override string Description => $"Add {m_traitID} trait";

		#endregion

		#region Constructors

		public AddAgentTrait(
			EffectBindingContext ctx,
			Agent agent,
			string traitID,
			int duration
		) : base(ctx, duration)
		{
			m_agent = agent;
			m_traitID = traitID;
		}

		#endregion

		#region Public Methods

		public override void Apply()
		{
			m_agent.AddTrait(m_traitID);
		}

		public override void Remove()
		{
			// Trait additions with durations are not permanent
			if (HasDuration)
			{
				m_agent.RemoveTrait(m_traitID);
			}
		}

		#endregion
	}
}
