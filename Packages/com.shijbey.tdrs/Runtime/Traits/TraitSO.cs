using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
		private string[] m_effects;

		[SerializeField]
		private SocialRuleEntry[] m_socialRules;

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

			HashSet<string> conflictingTraitIDs = new HashSet<string>(m_conflictingTraits);

			SocialRule[] socialRules = new SocialRule[m_socialRules.Length];

			for (int i = 0; i < m_socialRules.Length; i++)
			{
				SocialRuleEntry entry = m_socialRules[i];

				RePraxis.DBQuery precondition = null;

				if (entry.precondition != "")
				{
					precondition = new RePraxis.DBQuery(entry.precondition.Split("\n")
						.Where(clause => clause != "")
						.ToArray());
				}

				socialRules[i] = new SocialRule(
					precondition,
					entry.effects,
					entry.description
				);
			}


			var definition = new Trait(
				m_traitID,
				m_traitType,
				m_displayName,
				m_description,
				m_effects,
				socialRules,
				conflictingTraitIDs
			);

			for (int i = 0; i < socialRules.Length; i++)
			{
				socialRules[i].Source = definition;
			}

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
	}
}
