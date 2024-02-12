using System;

namespace TDRS
{
	/// <summary>
	/// Interface implemented by all effects that can be triggered by a social event, trait, or
	/// social rule.
	/// </summary>
	public interface IEffect
	{
		/// <summary>
		/// The object responsible for creating the effect.
		/// </summary>
		public object Source { get; set; }

		/// <summary>
		/// Does this effect have a duration.
		/// </summary>
		public bool HasDuration { get; }

		/// <summary>
		/// The amount of time remaining for this effect.
		/// </summary>
		public int RemainingDuration { get; }

		/// <summary>
		/// Should this effect still be active.
		/// </summary>
		public bool IsValid { get; }

		/// <summary>
		/// A text description of this effect.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// A description of why this effect is active.
		/// </summary>
		public string Cause { get; }

		/// <summary>
		/// Apply effects of the effect.
		/// </summary>
		public void Apply();

		/// <summary>
		/// Remove effects of the effect.
		/// </summary>
		public void Remove();

		/// <summary>
		/// Time tick this effect.
		/// </summary>
		public void Tick();
	}
}
