using System.Collections.Generic;

namespace TDRS.Serialization
{
	/// <summary>
	/// Applies stat changes to a relationship based on a set of preconditions.
	/// </summary>
	public class SerializedSocialRule
	{
		#region Properties

		/// <summary>
		/// A unique identifier for this social rule.
		/// </summary>
		public string ruleID { get; set; }

		/// <summary>
		/// A template description to be filled when recording the rules effects on a relationship.
		/// </summary>
		public string description { get; set; }

		/// <summary>
		/// RePraxis query clauses to run against the social engine's database.
		/// </summary>
		public string[] preconditions { get; set; }

		/// <summary>
		/// Effects to apply if the preconditions pass.
		/// </summary>
		public SerializedStatModifierData[] modifiers { get; set; }

		#endregion

		#region Constructors

		public SerializedSocialRule()
		{
			ruleID = "";
			description = "";
			preconditions = new string[0];
			modifiers = new SerializedStatModifierData[0];
		}

		#endregion

		#region PublicMethods

		public SocialRule ToRuntimeInstance()
		{
			var modifiers = new List<StatModifierData>();
			foreach (var serializedModifierData in this.modifiers)
			{
				modifiers.Add(
					new StatModifierData(
						serializedModifierData.statName,
						serializedModifierData.value,
						StatModifierType.Parse<StatModifierType>(
							serializedModifierData.modifierType, true
						)
					)
				);
			}

			return new SocialRule(
				ruleID, description, preconditions, modifiers.ToArray());
		}

		#endregion
	}
}
