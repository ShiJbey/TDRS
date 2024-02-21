using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDRS.Demo
{
	public class CharacterPanel : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private TMPro.TMP_Text m_nameText;

		[SerializeField]
		private TMPro.TMP_Text m_relationshipStatusText;

		[SerializeField]
		private TMPro.TMP_Text m_friendshipScoreText;

		[SerializeField]
		private AgentController m_agent;

		#endregion

		// Start is called before the first frame update
		void Start()
		{
			m_nameText.text = m_agent.UID;
		}

		// Update is called once per frame
		void Update()
		{
			if (SocialEngineController.Instance.State.HasRelationship(m_agent.UID, "player"))
			{
				var relationship = SocialEngineController.Instance.State.GetRelationship(
					m_agent.UID, "player");

				string relationshipStatus = "unknown";

				if (relationship.RelationshipType != null)
				{
					relationshipStatus = relationship.RelationshipType.DisplayName;
				}

				m_relationshipStatusText.text =
					$"Relationship Status: {relationshipStatus}";

				var friendship = relationship.Stats.GetStat("Friendship");

				m_friendshipScoreText.text =
					$"Friendship: {friendship.Value}/{friendship.MaxValue}";
			}

		}

	}
}
