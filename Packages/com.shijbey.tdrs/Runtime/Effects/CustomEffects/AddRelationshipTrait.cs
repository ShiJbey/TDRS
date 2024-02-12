namespace TDRS
{
	public class AddRelationshipTrait : Effect
	{
		#region Fields

		private Relationship m_relationship;
		private string m_traitID;

		#endregion

		#region Properties

		public override string Description
		{
			get
			{
				return $"Add {m_traitID} relationship trait.";
			}
		}

		#endregion

		#region Constructors

		public AddRelationshipTrait(
			EffectBindingContext ctx,
			Relationship relationship,
			string traitID,
			int duration
		) : base(ctx, duration)
		{
			m_relationship = relationship;
			m_traitID = traitID;
		}

		#endregion

		#region Public Methods

		public override void Apply()
		{
			m_relationship.AddTrait(m_traitID);
		}

		public override void Remove()
		{
			// Trait additions with durations are not permanent
			if (HasDuration)
			{
				m_relationship.RemoveTrait(m_traitID);
			}
		}

		#endregion
	}
}
