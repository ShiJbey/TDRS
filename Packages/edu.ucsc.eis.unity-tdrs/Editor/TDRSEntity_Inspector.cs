using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TDRS.Editor
{
	[CustomEditor(typeof(TDRSEntity))]
	public class TDRSEntity_Inspector : UnityEditor.Editor
	{
		public VisualTreeAsset m_InspectorXML;

		public override VisualElement CreateInspectorGUI()
		{
			VisualElement customInspector = new VisualElement();

			if (m_InspectorXML != null)
			{
				m_InspectorXML.CloneTree(customInspector);
			}
			else
			{
				VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
					"Packages/edu.ucsc.eis.unity-tdrs/Assets/TDRSEntity_Inspector_UXML.uxml"
				);
				visualTree.CloneTree(customInspector);
			}

			return customInspector;
		}
	}
}
