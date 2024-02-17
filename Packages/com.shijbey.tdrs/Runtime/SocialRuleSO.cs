using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace TDRS.Serialization
{
	/// <summary>
	/// Applies stat changes to a relationship based on a set of preconditions.
	/// </summary>
	[CreateAssetMenu(menuName = "TDRS/Social Rule")]
	public class SocialRuleSO : ScriptableObject
	{
		#region Properties

		/// <summary>
		/// A template description to be filled when recording the rules effects on a relationship.
		/// </summary>
		public string description;

		/// <summary>
		/// RePraxis query clauses to run against the social engine's database.
		/// </summary>
		public string[] preconditions;

		/// <summary>
		/// Effects to apply if the preconditions pass.
		/// </summary>
		public StatModifierEntry[] modifiers;

		#endregion

		#region Constructors

		public SocialRuleSO()
		{
			description = "";
			preconditions = new string[0];
			modifiers = new StatModifierEntry[0];
		}

		#endregion

		#region PublicMethods

		public SocialRule ToRuntimeInstance()
		{
			var modifiers = new List<StatModifierData>();
			foreach (var entry in this.modifiers)
			{
				modifiers.Add(
					new StatModifierData(
						entry.statName,
						entry.value,
						entry.modifierType
					)
				);
			}

			return new SocialRule(description, preconditions, modifiers.ToArray());
		}

		#endregion

		[Serializable]
		public class StatModifierEntry
		{
			/// <summary>
			/// The name of the stat to modify
			/// </summary>
			public string statName;

			/// <summary>
			/// The modifier value to apply.
			/// </summary>
			public float value;

			/// <summary>
			/// How to mathematically apply the modifier value.
			/// </summary>
			public StatModifierType modifierType = StatModifierType.FLAT;
		}
	}
}
