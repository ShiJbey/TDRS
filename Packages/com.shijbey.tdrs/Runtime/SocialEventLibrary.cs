using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// Manages the collection of social event definitions.
	/// </summary>
	public class SocialEventLibrary
	{
		#region Fields

		/// <summary>
		/// Event definitions sorted by name and cardinality.
		/// </summary>
		protected Dictionary<string, SocialEvent> m_events;

		#endregion

		#region Constructors

		public SocialEventLibrary()
		{
			m_events = new Dictionary<string, SocialEvent>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a social event to the library.
		/// </summary>
		/// <param name="socialEvent"></param>
		public void AddSocialEvent(SocialEvent socialEvent)
		{
			m_events[socialEvent.Symbol] = socialEvent;
		}

		/// <summary>
		/// Get an event type by symbol (i.e., "eventName/#")
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public SocialEvent GetSocialEvent(string symbol)
		{
			return m_events[symbol];
		}

		#endregion
	}
}
