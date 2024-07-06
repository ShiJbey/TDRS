using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// Definition information for creating SocialEvent instances
	/// </summary>
	public class SocialEvent
	{
		#region Fields

		protected string m_name;
		protected string[] m_roles;
		protected string m_description;
		protected SocialEventResponse[] m_responses;
		protected SocialEventTriggerRule[] m_triggerRules;

		#endregion

		#region Properties

		public string Name => m_name;
		public string[] Roles => m_roles;
		public string Description => m_description;
		public SocialEventResponse[] Responses => m_responses;
		public int Cardinality => m_roles.Length;
		public string Symbol => $"{m_name}/{Cardinality}";
		public SocialEventTriggerRule[] TriggerRules => m_triggerRules;

		#endregion

		#region Constructors

		public SocialEvent(
			string name,
			string[] roles,
			string description,
			SocialEventResponse[] responses,
			SocialEventTriggerRule[] triggerRules = null
		)
		{
			m_name = name;
			m_roles = roles;
			m_description = description;
			m_responses = responses;
			m_triggerRules = (
				(triggerRules != null) ? triggerRules : new SocialEventTriggerRule[0]
			);
		}

		#endregion

		#region Methods

		public override string ToString()
		{
			return Symbol;
		}

		#endregion
	}
}
