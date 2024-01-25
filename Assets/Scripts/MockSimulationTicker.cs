using TDRS;
using UnityEngine;

/// <summary>
/// Ticks the simulation by one step when a button is pressed
/// </summary>
public class MockSimulationTicker : MonoBehaviour
{
	public enum TickButtonCode
	{
		Tab = KeyCode.Tab,
	}

	[SerializeField]
	private TickButtonCode m_tickButton = TickButtonCode.Tab;

	[SerializeField]
	private SocialEngine m_socialEngine;

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyUp((KeyCode)m_tickButton))
		{
			m_socialEngine.Tick();
		}
	}
}
