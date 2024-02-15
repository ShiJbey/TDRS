using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// A collection of effects to be applied and removed as a group.
	/// </summary>
	public class EffectGroup : IEffect
	{
		/// <summary>
		/// The target of this effect.
		/// </summary>
		public IEffectable Target { get; }

		public EffectContext Context { get; protected set; }

		public virtual IEffectSource Source => Context.Source;

		public bool HasDuration { get; protected set; }

		public int RemainingDuration { get; protected set; }

		/// <summary>
		/// Should this effect be tracked in the effect manager, or is it a one off thing.
		/// </summary>
		public bool IsPersistent { get; }

		public virtual bool IsValid
		{
			get
			{
				return !HasDuration || (HasDuration && RemainingDuration > 0);
			}
		}

		public bool IsActive { get; set; }

		public string Description { get; }

		public virtual string Cause => Context.CauseDescription;

		public List<IEffect> Effects { get; protected set; }

		public EffectGroup(
			EffectContext ctx,
			int duration
		)
		{
			Context = ctx;
			HasDuration = duration == -1;
			RemainingDuration = duration;
			Effects = new List<IEffect>();
		}

		/// <summary>
		/// Apply the effects associated with this group.
		/// </summary>
		public void Apply()
		{
			foreach (var effect in Effects)
			{
				if (!effect.IsActive)
				{
					effect.Apply();
					effect.IsActive = true;
				}
			}
		}

		/// <summary>
		/// Undo the effects associated with this group.
		/// </summary>
		public void Remove()
		{
			foreach (var effect in Effects)
			{
				if (effect.IsActive)
				{
					effect.Remove();
					effect.IsActive = false;
				}
			}
		}

		/// <summary>
		/// Tick all effects in the group.
		/// </summary>
		public void Tick()
		{
			foreach (var effect in Effects)
			{
				effect.Tick();
			}
		}
	}
}
