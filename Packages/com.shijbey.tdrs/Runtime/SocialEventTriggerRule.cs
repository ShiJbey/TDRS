using System.Collections.Generic;

namespace TDRS
{
	public class SocialEventTriggerRule
	{
		#region Properties

		/// <summary>
		/// If False, this event may only automatically trigger once.
		/// </summary>
		public bool IsRepeatable { get; set; }
		/// <summary>
		/// Max number of times we can trigger this event per simulation tick.
		/// </summary>
		public int MaxUsesPerTick { get; set; }
		/// <summary>
		/// The required number of simulation ticks between consecutive uses.
		/// The event can still trigger multiple times within the same time step.
		/// </summary>
		public int Cooldown { get; set; }
		/// <summary>
		/// Amount of simulation ticks before this rule is eligible again.
		/// </summary>
		public int CooldownTimeRemaining { get; private set; }
		/// <summary>
		/// Has the trigger rule been used previously.
		/// </summary>
		public int Uses { get; set; }
		/// <summary>
		/// Database precondition queries used to automatically cast agents into
		/// the social event. The results of all preconditions are joined to get
		/// all valid permutations of the event.
		/// </summary>
		public string[] Preconditions { get; set; }

		#endregion

		#region Constructors

		public SocialEventTriggerRule()
		{
			IsRepeatable = true;
			MaxUsesPerTick = int.MaxValue;
			Cooldown = -1;
			Preconditions = new string[0];
			Uses = 0;
		}

		#endregion

		#region Public Methods

		public void ResetCooldownTimer()
		{
			CooldownTimeRemaining = Cooldown;
		}

		public void DecrementCooldownTimer()
		{
			CooldownTimeRemaining -= 1;
		}

		public bool IsEligible()
		{
			bool hasCooldown = Cooldown > 0;

			if (hasCooldown && CooldownTimeRemaining > 0) return false;

			if (Uses > 0 && !IsRepeatable) return false;

			return true;
		}

		#endregion
	}
}
