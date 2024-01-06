namespace TDRS
{
	/// <summary>
	/// Interface implemented by all factory classes that create social event effects
	/// </summary>
	public interface ISocialEventEffectFactory
	{
		/// <summary>
		///  Get the type of effect this factory produces
		/// </summary>
		public string EffectType { get; }

		/// <summary>
		/// Create a new social event effect instance
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public ISocialEventEffect CreateInstance(SocialEventContext context, params string[] args);
	}
}
