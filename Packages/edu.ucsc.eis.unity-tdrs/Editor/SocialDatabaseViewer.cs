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
		private TDRSManager tdrsManager;


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

			tdrsManager = FindObjectOfType<TDRSManager>();

			if (tdrsManager is null)
			{
				Debug.LogError(
					"Database Viewer cannot find GameObject with TDRSManager component.");
			}
		}

		public void Update()
		{
			scrollView.Clear();

			var nodeStack = new Stack<INode>(tdrsManager.SocialEngine.DB.Root.Children);

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
