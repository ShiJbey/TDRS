using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using RePraxis;
using TDRS;

namespace Calypso
{
	public class SocialDatabaseViewer : EditorWindow
	{
		private ScrollView scrollView;
		private SocialEngine socialEngine;


		[MenuItem("Window/TDRS/Database Viewer")]
		public static void ShowWindow()
		{
			SocialDatabaseViewer window = GetWindow<SocialDatabaseViewer>();
			window.titleContent = new GUIContent("Database Viewer");
		}

		public void CreateGUI()
		{
			VisualElement root = rootVisualElement;

			scrollView = new ScrollView(ScrollViewMode.Vertical);

			root.Add(scrollView);

			socialEngine = FindObjectOfType<SocialEngine>();

			if (socialEngine == null)
			{
				Debug.LogError(
					"Database Viewer cannot find GameObject with SocialEngine component.");
			}
		}

		public void Update()
		{
			scrollView.Clear();

			if (socialEngine == null || socialEngine.DB == null) return;

			var nodeStack = new Stack<INode>(socialEngine.DB.Root.Children);

			while (nodeStack.Count > 0)
			{
				INode node = nodeStack.Pop();

				IEnumerable<INode> children = node.Children;

				if (children.Count() > 0)
				{
					// Add children to the stack
					foreach (var child in children)
					{
						nodeStack.Push(child);
					}
				}
				else
				{
					// This is a leaf
					scrollView.Add(new Label(node.GetPath()));
				}
			}
		}
	}
}
