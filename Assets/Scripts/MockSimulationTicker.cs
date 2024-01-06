using TDRS;
using UnityEngine;

public class MockSimulationTicker : MonoBehaviour
{
	[SerializeField]
	private SocialEngine m_socialEngine;

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Tab))
		{
			m_socialEngine.Tick();
		}
	}
}
