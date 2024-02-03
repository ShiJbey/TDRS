using UnityEngine;
using TDRS;

/// <summary>
/// This class is a stand in for an actual game manager or content loader that someone would place
/// in their scene. The execution order is shifted to a later priority to ensure that all other
/// components have been properly initialized before this one runs the Start method.
/// </summary>
[DefaultExecutionOrder(5)]
public class MockGameManager : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		SocialEngine.Instance.Initialize();
	}
}
