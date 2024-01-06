namespace TDRS
{
	/// <summary>
	/// Interface implemented by all effects that can be triggered by a SocialEvent
	/// </summary>
	public interface ISocialEventEffect
	{
		/// <summary>
		/// Apply effects of the effect
		/// </summary>
		/// <param name="context"></param>
		public void Apply();
	}
}
