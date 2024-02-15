namespace TDRS
{
	/// <summary>
	/// An abstract class to facilitate creating new IEffect types.
	/// </summary>
	public abstract class Effect : IEffect
	{
		#region Properties

		public IEffectable Target { get; }

		public EffectContext Context { get; protected set; }

		public IEffectSource Source => Context.Source;

		public bool IsPersistent { get; }

		public bool HasDuration { get; protected set; }

		public int RemainingDuration { get; protected set; }

		public virtual bool IsValid
		{
			get
			{
				return !HasDuration || (HasDuration && RemainingDuration > 0);
			}
		}

		public bool IsActive { get; set; }

		public abstract string Description { get; }

		public virtual string Cause => Context.CauseDescription;

		#endregion

		#region Constructors

		public Effect(
			IEffectable target,
			EffectContext ctx,
			int duration
		)
		{
			Target = target;
			Context = ctx;
			HasDuration = duration > 0;
			RemainingDuration = duration;
			IsActive = false;
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
