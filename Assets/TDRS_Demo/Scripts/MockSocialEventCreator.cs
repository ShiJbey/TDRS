using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TDRS.Demo
{
	/// <summary>
	/// Mock simulates a social event being emitted by some game system
	/// </summary>
	public class MockSocialEventCreator : MonoBehaviour
	{
		[SerializeField]
		private string m_eventType;

		[SerializeField]
		private List<AgentController> m_agents;

		[SerializeField]
		private KeyCode m_fireEventButton = KeyCode.Space;

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyUp(m_fireEventButton))
			{
				SocialEngineController.Instance.State.DispatchEvent(m_eventType, m_agents.Select(a => a.UID).ToArray());
			}
		}
	}

}
