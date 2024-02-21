using System;
using System.Linq;
using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// A ScriptableObject tpe for creating social event definitions
	/// within Unity's editor
	/// </summary>
	[CreateAssetMenu(menuName = "TDRS/Social Event")]
	public class SocialEventSO : ScriptableObject
	{
		[SerializeField]
		private string m_eventName;

		[SerializeField]
		private string[] m_roles;

		[SerializeField]
		private string m_description;

		[SerializeField]
		private EventResponseEntry[] m_responses;

		public SocialEvent GetSocialEvent()
		{
			if (m_eventName == "")
			{
				throw new ArgumentException($"socialEventSO '{name}' is missing a name");
			}

			if (m_description == "")
			{
				throw new ArgumentException($"socialEventSO '{name}' is missing a description");
			}

			SocialEventResponse[] responses = new SocialEventResponse[m_responses.Length];
			for (int i = 0; i < m_responses.Length; i++)
			{
				EventResponseEntry entry = m_responses[i];

				string[] preconditions = new string[0];

				if (entry.precondition != "")
				{
					preconditions = entry.precondition.Split("\n")
						.Where(clause => clause != "")
						.ToArray();
				}

				responses[i] = new SocialEventResponse(
					preconditions,
					entry.effects,
					entry.description
				);
			}

			var socialEvent = new SocialEvent(
				name,
				m_roles,
				m_description,
				responses
			);

			return socialEvent;
		}

		[Serializable]
		public class EventResponseEntry
		{
			[TextArea(minLines: 3, maxLines: 8)]
			public string precondition;
			public string[] effects;
			public string description;
		}
	}
}
