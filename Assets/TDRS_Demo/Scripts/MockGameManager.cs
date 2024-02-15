using System.IO;
using TDRS.Serialization;
using UnityEngine;

namespace TDRS.Demo
{
	/// <summary>
	/// This class is a stand in for an actual game manager or content loader that someone would
	/// place in their scene. The execution order is shifted to a later priority to ensure that all
	/// other components have been properly initialized before this one runs the Start method.
	/// </summary>
	[DefaultExecutionOrder(5)]
	public class MockGameManager : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start()
		{
			// Load initial content into the social engine

			SocialEngineController.Instance.Initialize();

			// You need to initialize the Social engine before you load the save file
			// because it depends on the agent and relationship configs supplied
			// in the inspector and/or loaded from StreamingAssets.

			string filePath = Path.Combine(
				Application.persistentDataPath,
				MockSaveSystem.SAVE_PATH
			);

			if (File.Exists(filePath))
			{
				string yamlData = File.ReadAllText(filePath);
				SerializedSocialEngine.Deserialize(SocialEngineController.Instance.State, yamlData);
				Debug.Log($"Loaded save from: {filePath}");
			}

			// Once the save file is loaded, register the agent and relationship
			// GameObjects in the scene.

			SocialEngineController.Instance.RegisterAgentsAndRelationships();
		}
	}
}
