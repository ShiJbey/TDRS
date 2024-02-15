using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// Manages all the social rules associated with an agent.
	/// </summary>
	public class SocialRuleManager
	{
		#region Fields

		/// <summary>
		/// All the social rule sources added to the manager.
		/// </summary>
		protected List<ISocialRuleSource> m_sources;

		#endregion

		#region Properties

		/// <summary>
		/// All the social rule sources added to the manager.
		/// </summary>
		public IEnumerable<ISocialRuleSource> Sources => m_sources;

		#endregion

		#region Constructors

		public SocialRuleManager()
		{
			m_sources = new List<ISocialRuleSource>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a source to the manager.
		/// </summary>
		/// <param name="source"></param>
		public void AddSource(ISocialRuleSource source)
		{
			m_sources.Add(source);
		}

		/// <summary>
		/// Remove a source from the manager.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public bool RemoveSource(ISocialRuleSource source)
		{
			return m_sources.Remove(source);
		}

		#endregion
	}
}
