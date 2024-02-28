using System.IO;
using UnityEngine;
using System.Collections.Generic;
using TDRS.Serialization;

namespace TDRS.Demo
{
	/// <summary>
	/// Sample script that shows how to save the state of the social engine.
	/// <para>
	/// The <c>MockGameManager</c> shows how to load the save file.
	/// </para>
	/// </summary>
	public class MockSaveSystem : MonoBehaviour
	{
		public class IngredientList
		{
			public List<string> ingredients;
		}

		public const string SAVE_PATH = "TDRS_demo_save.yaml";

		[SerializeField]
		private KeyCode m_saveButton = KeyCode.S;

		[SerializeField]
		private KeyCode m_deleteSaveButton = KeyCode.Backspace;

		// Update is called once per frame
		void Update()
		{
			// Create a new save

			if (Input.GetKeyUp(m_saveButton))
			{
				var jsonExporter = new TdrsJsonExporter();

				string jsonString = jsonExporter.Export(
					SocialEngineController.Instance.State
				);

				string filePath = Path.Combine(Application.persistentDataPath, SAVE_PATH);

				File.WriteAllText(filePath, jsonString);

				Debug.Log($"Saved state to: {filePath}");
			}

			// Delete the existing save

			if (Input.GetKeyUp(m_deleteSaveButton))
			{
				string filePath = Path.Combine(Application.persistentDataPath, SAVE_PATH);

				File.Delete(filePath);

				Debug.Log($"Deleted save file at: {filePath}");
			}
		}
	}
}
