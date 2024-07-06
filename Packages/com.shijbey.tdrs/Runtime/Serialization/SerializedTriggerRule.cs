using System.Collections.Generic;

namespace TDRS.Serialization
{
	public class SerializedTriggerRule
	{
		#region Properties

		public bool isRepeatable { get; set; }
		public int maxUsesPerTick { get; set; }
		public int cooldown { get; set; }
		public string[] preconditions { get; set; }

		#endregion

		#region Constructors

		public SerializedTriggerRule()
		{
			isRepeatable = false;
			maxUsesPerTick = int.MaxValue;
			cooldown = 0;
			preconditions = new string[0];
		}

		#endregion

		#region Public Methods

		public SocialEventTriggerRule ToRuntimeInstance()
		{
			return new SocialEventTriggerRule()
			{
				IsRepeatable = isRepeatable,
				MaxUsesPerTick = maxUsesPerTick,
				Cooldown = cooldown,
				Preconditions = preconditions
			};
		}

		#endregion
	}
}
