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
		protected Dictionary<string, SocialEventType> m_events;

		#endregion

		public Dictionary<string, SocialEventType> Events => m_events;

		#region Constructors

		public SocialEventLibrary()
		{
			m_events = new Dictionary<string, SocialEventType>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a social event to the library.
		/// </summary>
		/// <param name="socialEvent"></param>
		public void AddSocialEvent(SocialEventType socialEvent)
		{
			m_events[socialEvent.Symbol] = socialEvent;
		}

		/// <summary>
		/// Get an event type by symbol (i.e., "eventName/#")
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public SocialEventType GetSocialEvent(string symbol)
		{
			return m_events[symbol];
		}

		#endregion
	}
}
