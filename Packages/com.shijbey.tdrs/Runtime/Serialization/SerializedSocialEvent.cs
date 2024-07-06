using System.Linq;

namespace TDRS.Serialization
{
	public class SerializedSocialEvent
	{
		#region Properties

		public string name { get; set; }
		public string[] roles { get; set; }
		public string description { get; set; }
		[YamlDotNet.Serialization.YamlMember(DefaultValuesHandling = YamlDotNet.Serialization.DefaultValuesHandling.OmitNull)]
		public SerializedTriggerRule[] triggerRules { get; set; }
		public SerializedSocialEventResponse[] responses { get; set; }

		#endregion

		#region Constructors

		public SerializedSocialEvent()
		{
			name = "";
			roles = new string[0];
			description = "";
			triggerRules = new SerializedTriggerRule[0];
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
							response.effects,
							response.description
						);
					})
					.ToArray(),
				triggerRules: triggerRules.Select(r => r.ToRuntimeInstance()).ToArray()
			);
		}

		#endregion
	}
}
