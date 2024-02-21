using UnityEngine;

namespace TDRS.Demo
{
	/// <summary>
	/// Ticks the simulation by one step when a button is pressed
	/// </summary>
	public class MockSimulationTicker : MonoBehaviour
	{
		[SerializeField]
		private KeyCode m_tickButton = KeyCode.Tab;

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyUp(m_tickButton))
			{
				SocialEngineController.Instance.State.Tick();
			}
		}
	}

}
