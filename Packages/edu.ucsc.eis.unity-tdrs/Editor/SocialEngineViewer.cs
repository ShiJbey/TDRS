using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TDRS
{
	public class SocialEngineViewer : EditorWindow
	{
		private ScrollView scrollView;
		private SocialEngine manager;

		[MenuItem("Window/TDRS/Social Engine Viewer")]
		public static void ShowWindow()
		{
			SocialEngineViewer window = GetWindow<SocialEngineViewer>();
			window.titleContent = new GUIContent("Social Engine Viewer");
			window.Show();
		}

		public void CreateGUI()
		{
			VisualElement root = rootVisualElement;

			scrollView = new ScrollView(ScrollViewMode.Vertical);

			root.Add(scrollView);

			manager = FindObjectOfType<SocialEngine>();

			if (manager == null) throw new Exception("Cannot find TDRSManager in scene");

			foreach (var node in manager.Nodes)
			{
				scrollView.Add(new Label(node.UID));
			}
		}

		public void Update()
		{
			scrollView.Clear();

			foreach (var node in manager.Nodes)
			{
				scrollView.Add(new Label(node.UID));
			}

			// var columns = new Columns();
			// columns.Add(new Column() { makeHeader = () => new Label("UID") });
			// columns.Add(new Column() { makeHeader = () => new Label("Traits") });

			// scrollView.Add(columns);

			// scrollView.Add(new MultiColumnController(columns, new SortColumnDescriptions(), new List<SortColumnDescription>()));

			// foreach (var node in manager.SocialEngine.Nodes)
			// {



			// 	idColumn.

			// 	columns.Add(new Column()node.UID));
			// }
		}
	}
}
