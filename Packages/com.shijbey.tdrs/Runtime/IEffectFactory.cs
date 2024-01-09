namespace TDRS
{
	/// <summary>
	/// Interface implemented by all factory classes that create social event effects
	/// </summary>
	public interface IEffectFactory
	{
		/// <summary>
		///  Get the type of effect this factory produces
		/// </summary>
		public string EffectName { get; }

		/// <summary>
		/// Create a new social event effect instance
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public IEffect CreateInstance(
			EffectBindingContext context, params string[] args
		);
	}
}
