namespace TDRS
{
	public class RemoveRelationshipTrait : Effect
	{
		#region Fields

		protected Relationship m_relationship;
		protected string m_traitID;

		#endregion

		#region Properties

		public override string Description => $"Remove {m_traitID} relationship trait";

		#endregion

		#region Constructors

		public RemoveRelationshipTrait(
			EffectBindingContext ctx,
			Relationship relationship,
			string traitID
		) : base(ctx, -1)
		{
			m_relationship = relationship;
			m_traitID = traitID;
		}

		#endregion

		#region Public Methods

		public override void Apply()
		{
			m_relationship.RemoveTrait(m_traitID);
		}

		public override void Remove()
		{
			// trait removal is permanent
			return;
		}

		#endregion
	}
}
