namespace TDRS
{
	public class SocialEventResponse
	{
		#region Properties

		public string[] Preconditions { get; }
		public string[] Effects { get; }
		public string Description { get; }

		#endregion

		#region Constructors

		public SocialEventResponse(string[] preconditions, string[] effects, string description)
		{
			Preconditions = preconditions;
			Effects = effects;
			Description = description;
		}

		#endregion
	}
}
