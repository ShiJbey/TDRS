using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// A record of the application of a social rule definition. It maintains what bindings were
	/// involved in the social rule and the instances of the effects that were applied
	/// </summary>
	public class SocialRuleInstance
	{
		#region Fields

		protected Dictionary<string, string> m_bindings;
		protected ISocialEventEffect[] m_effects;
		protected SocialRuleDefinition m_source;

		#endregion

		#region Properties

		/// <summary>
		/// The bindings used to instantiate the associated effects.
		/// </summary>
		public Dictionary<string, string> Bindings => m_bindings;
		/// <summary>
		/// The instantiated effects created from the bindings.
		/// </summary>
		public ISocialEventEffect[] Effects => m_effects;
		/// <summary>
		/// The social rule responsible for this instance.
		/// </summary>
		public SocialRuleDefinition Source => m_source;

		#endregion

		#region Constructors

		public SocialRuleInstance(
			Dictionary<string, string> bindings,
			ISocialEventEffect[] effects,
			SocialRuleDefinition source
		)
		{
			m_bindings = bindings;
			m_effects = effects;
			m_source = source;
		}

		#endregion
	}
}
