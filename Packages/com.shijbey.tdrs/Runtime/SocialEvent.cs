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
		protected string m_descriptionTemplate;
		protected SocialEventResponse[] m_responses;

		#endregion

		#region Properties

		public string Name => m_name;
		public string[] Roles => m_roles;
		public string DescriptionTemplate => m_descriptionTemplate;
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
			m_descriptionTemplate = description;
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

	public class SocialEventResponse
	{
		#region Properties

		public string[] Preconditions { get; set; }
		public string[] Effects { get; set; }

		#endregion

		#region Constructors

		public SocialEventResponse(string[] preconditions, string[] effects)
		{
			Preconditions = preconditions;
			Effects = effects;
		}

		#endregion
	}
}
