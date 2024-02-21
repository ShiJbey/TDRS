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

		#endregion

		#region Properties

		public string Name => m_name;
		public string[] Roles => m_roles;
		public string Description => m_description;
		public SocialEventResponse[] Responses => m_responses;
		public int Cardinality => m_roles.Length;
		public string Symbol => $"{m_name}/{Cardinality}";

		#endregion

		#region Constructors

		public SocialEvent(
			string name,
			string[] roles,
			string description,
			SocialEventResponse[] responses
		)
		{
			m_name = name;
			m_roles = roles;
			m_description = description;
			m_responses = responses;
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
