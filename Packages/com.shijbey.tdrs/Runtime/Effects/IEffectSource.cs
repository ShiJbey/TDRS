namespace TDRS
{
	/// <summary>
	/// Interface implemented by any class that can be the source of an <c>IEffect</c> applied to
	/// an agent or relationship.
	/// </summary>
	public interface IEffectSource
	{
		/// <summary>
		/// A unique string identifier used to connect a source to an effect during serialization.
		/// </summary>
		public string EffectSourceID { get; }
	}
}
