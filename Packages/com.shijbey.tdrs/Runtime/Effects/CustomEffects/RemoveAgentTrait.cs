namespace TDRS
{
	public class RemoveAgentTrait : Effect
	{
		#region Fields

		protected Agent m_agent;
		protected string m_traitID;

		#endregion

		#region Properties

		public override string Description => $"Remove {m_traitID} trait.";

		#endregion

		#region Constructors

		public RemoveAgentTrait(
			EffectBindingContext ctx,
			Agent agent,
			string traitID
		) : base(ctx, -1)
		{
			m_agent = agent;
			m_traitID = traitID;
		}

		#endregion

		#region Public Methods

		public override void Apply()
		{
			m_agent.RemoveTrait(m_traitID);
		}

		public override void Remove()
		{
			// Trait removal is permanent
			return;
		}

		#endregion
	}
}
