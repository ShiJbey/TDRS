namespace TDRS
{
	/// <summary>
	/// An abstract class to facilitate creating new IEffect types.
	/// </summary>
	public abstract class Effect : IEffect
	{
		#region Properties

		public EffectBindingContext Context { get; protected set; }

		public virtual object Source { get; set; }

		public bool HasDuration { get; protected set; }

		public int RemainingDuration { get; protected set; }

		public virtual bool IsValid
		{
			get
			{
				return !HasDuration || (HasDuration && RemainingDuration > 0);
			}
		}

		public abstract string Description { get; }

		public virtual string Cause => Context.Description;

		#endregion

		#region Constructors

		public Effect(
			EffectBindingContext ctx,
			int duration
		)
		{
			Context = ctx;
			HasDuration = duration == -1;
			RemainingDuration = duration;
		}

		#endregion

		#region Public Methods

		public abstract void Apply();

		public abstract void Remove();

		public void Tick()
		{
			if (HasDuration)
			{
				RemainingDuration -= 1;
			}
		}

		public override string ToString()
		{
			return Description;
		}

		#endregion
	}
}
