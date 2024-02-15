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
			EffectContext ctx,
			Agent agent,
			string traitID,
			int duration
		) : base(agent, ctx, duration)
		{
			m_agent = agent;
			m_traitID = traitID;
		}

		#endregion

		#region Public Methods

		public override void Apply()
		{
			m_agent.AddTrait(m_traitID);
			m_agent.Effects.AddEffect(this);
		}

		public override void Remove()
		{
			m_agent.RemoveTrait(m_traitID);
			m_agent.Effects.RemoveEffect(this);
		}

		#endregion
	}
}
