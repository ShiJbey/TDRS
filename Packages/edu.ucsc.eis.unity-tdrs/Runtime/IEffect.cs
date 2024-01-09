namespace TDRS
{
	/// <summary>
	/// Interface implemented by all effects that can be triggered by a social event, trait, or
	/// social rule.
	/// </summary>
	public interface IEffect
	{
		/// <summary>
		/// Apply effects of the effect
		/// </summary>
		public void Apply();

		/// <summary>
		/// Remove effects of the effect
		/// </summary>
		public void Remove();
	}
}
