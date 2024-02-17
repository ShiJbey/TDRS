namespace TDRS.Serialization
{
	public class SerializedSocialEventResponse
	{
		#region Properties

		public string[] preconditions { get; set; }
		public string[] effects { get; set; }
		public string description { get; set; }

		#endregion

		#region Constructors

		public SerializedSocialEventResponse()
		{
			preconditions = new string[0];
			effects = new string[0];
			description = "";
		}

		#endregion
	}
}
