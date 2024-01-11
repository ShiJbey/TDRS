using System.Collections.Generic;
using UnityEngine;
using TDRS;
using System.Linq;

/// <summary>
/// Mock simulates a social event being emitted by some game system
/// </summary>
public class MockSocialEventCreator : MonoBehaviour
{
	[SerializeField]
	private SocialEngine m_socialEngine;

	[SerializeField]
	private string m_eventType;

	[SerializeField]
	private List<SocialAgent> m_agents;

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Space))
		{
			m_socialEngine.DispatchEvent(m_eventType, m_agents.Select(a => a.UID).ToArray());
		}
	}
}
