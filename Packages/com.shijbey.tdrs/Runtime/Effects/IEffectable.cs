namespace TDRS
{
	/// <summary>
	/// An interface for any object that can be the target of an effect.
	/// </summary>
	public interface IEffectable
	{
		/// <summary>
		/// Manages all effects applied to this object.
		/// </summary>
		public EffectManager Effects { get; }
	}
}
