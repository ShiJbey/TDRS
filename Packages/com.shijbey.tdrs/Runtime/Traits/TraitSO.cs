using System;
using System.Collections.Generic;
using UnityEngine;
using TDRS.Serialization;

namespace TDRS
{
	/// <summary>
	/// A ScriptableObject type for creating trait definitions within
	/// Unity's editor
	/// </summary>
	[CreateAssetMenu(menuName = "TDRS/Trait")]
	public class TraitSO : ScriptableObject
	{
		[SerializeField]
		private string m_traitID;

		[SerializeField]
		private TraitType m_traitType;

		[SerializeField]
		private string m_displayName;

		[SerializeField]
		private string m_description;

		[SerializeField]
		private List<StatModifier> m_modifiers;

		[SerializeField]
		private string[] m_conflictingTraits;

		public Trait CreateTrait()
		{
			if (m_traitID == "")
			{
				throw new ArgumentException($"TraitSO '{name}' is missing a traitID");
			}

			if (m_displayName == "")
			{
				throw new ArgumentException($"TraitSO '{name}' is missing a displayName");
			}

			if (m_description == "")
			{
				throw new ArgumentException($"TraitSO '{name}' is missing a description");
			}

			var modifiers = new List<StatModifierData>();
			foreach (var entry in m_modifiers)
			{
				modifiers.Add(
					new StatModifierData(
						entry.statName,
						entry.value,
						entry.modifierType
					)
				);
			}

			var definition = new Trait(
				m_traitID,
				m_traitType,
				m_displayName,
				m_description,
				modifiers,
				m_conflictingTraits
			);

			return definition;
		}

		[Serializable]
		public class SocialRuleEntry
		{
			[TextArea(minLines: 3, maxLines: 8)]
			public string precondition;
			public string[] effects;
			public string description;
		}

		[Serializable]
		public class StatModifier
		{
			/// <summary>
			/// The name of the stat to modify
			/// </summary>
			[SerializeField]
			public string statName;

			/// <summary>
			/// The modifier value to apply.
			/// </summary>
			public float value;

			/// <summary>
			/// How to mathematically apply the modifier value.
			/// </summary>
			public StatModifierType modifierType;
		}
	}
}
