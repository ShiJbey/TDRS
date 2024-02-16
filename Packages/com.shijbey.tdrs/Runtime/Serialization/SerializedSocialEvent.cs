using System.Linq;

namespace TDRS.Serialization
{
	public class SerializedSocialEvent
	{
		#region Properties

		public string name { get; set; }
		public string[] roles { get; set; }
		public string description { get; set; }
		public SerializedSocialEventResponse[] responses { get; set; }

		#endregion

		#region Constructors

		public SerializedSocialEvent()
		{
			name = "";
			roles = new string[0];
			description = "";
			responses = new SerializedSocialEventResponse[0];
		}

		#endregion

		#region Public Methods

		public SocialEvent ToRuntimeInstance()
		{
			return new SocialEvent(
				name: name,
				roles: roles,
				description: description,
				responses: responses
					.Select(response =>
					{
						return new SocialEventResponse(
							response.preconditions,
							response.effects
						);
					})
					.ToArray()
			);
		}

		#endregion
	}
}
