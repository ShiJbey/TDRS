using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace TDRS
{
	[CustomEditor(typeof(AgentController))]
	public class AgentControllerEditor : Editor
	{
		public VisualTreeAsset m_UXml;

		private VisualElement m_root;
		private List<TraitInstance> m_traitsList;
		private List<KeyValuePair<string, Stat>> m_statsList;
		private AgentController m_agent;
		private MultiColumnListView m_traitListView;
		private MultiColumnListView m_statsListView;

		public override VisualElement CreateInspectorGUI()
		{
			m_agent = target as AgentController;
			m_root = new VisualElement();
			m_UXml.CloneTree(m_root);

			CreateTraitGUI(m_root);
			CreateStatsGUI(m_root);

			return m_root;
		}

		private void CreateTraitGUI(VisualElement root)
		{
			m_traitsList = new List<TraitInstance>();
			m_traitListView = root.Q<MultiColumnListView>(name: "Traits");


			if (m_agent.Agent != null)
			{
				m_traitsList = m_agent.Agent.Traits.Traits.ToList();
			}

			m_traitListView.itemsSource = m_traitsList;

			var cols = m_traitListView.columns;

			// Set makeCell

			cols["trait"].makeCell = () => new Label();
			cols["description"].makeCell = () => new TextElement();
			cols["modifiers"].makeCell = () => new TextElement();

			// Set bindCell

			cols["trait"].bindCell = (VisualElement e, int index) =>
			{
				(e as Label).text = m_traitsList[index].DisplayName;
			};

			cols["description"].bindCell = (VisualElement e, int index) =>
			{
				(e as TextElement).text = m_traitsList[index].Description;
			};

			cols["modifiers"].bindCell = (VisualElement e, int index) =>
			{
				List<string> modifierDescriptions = new List<string>();

				var traitInstance = m_traitsList[index];
				foreach (var entry in traitInstance.Modifiers)
				{
					modifierDescriptions.Add(
						$"{entry.StatName}: {entry.Value} ({entry.ModifierType})"
					);
				}

				(e as TextElement).text = string.Join("\n", modifierDescriptions);
			};
		}

		private void CreateStatsGUI(VisualElement root)
		{
			m_statsList = new List<KeyValuePair<string, Stat>>();

			m_statsListView = root.Q<MultiColumnListView>(name: "Stats");

			if (m_agent.Agent != null)
			{
				m_statsList = m_agent.Agent.Stats.Stats.ToList();
			}

			m_statsListView.itemsSource = m_statsList;

			var cols = m_statsListView.columns;

			// Set makeCell

			cols["stat"].makeCell = () => new Label();
			cols["value"].makeCell = () => new TextElement();

			// Set bindCell

			cols["stat"].bindCell = (VisualElement e, int index) =>
			{
				(e as Label).text = m_statsList[index].Key;
			};

			cols["value"].bindCell = (VisualElement e, int index) =>
			{
				Stat stat = m_statsList[index].Value;
				(e as TextElement).text = $"{stat.Value}({stat.BaseValue})";
			};
		}

		public void OnEnable()
		{
			m_agent = target as AgentController;


			m_agent.OnTick.AddListener(HandleOnTick);
			m_agent.OnStatChange.AddListener(HandleOnStatChange);
			m_agent.OnTraitAdded.AddListener(HandleTraitUpdate);
			m_agent.OnTraitRemoved.AddListener(HandleTraitUpdate);
			m_agent.OnRegistered.AddListener(HandleOnRegistered);
		}

		public void OnDisable()
		{
			m_agent.OnTick.RemoveListener(HandleOnTick);
			m_agent.OnStatChange.RemoveListener(HandleOnStatChange);
			m_agent.OnTraitAdded.RemoveListener(HandleTraitUpdate);
			m_agent.OnTraitRemoved.RemoveListener(HandleTraitUpdate);
			m_agent.OnRegistered.RemoveListener(HandleOnRegistered);
		}

		private void HandleOnRegistered()
		{
			UpdateGUI();
		}


		private void HandleOnTick()
		{
			UpdateGUI();
		}

		private void HandleOnStatChange(string stat, float value)
		{
			UpdateGUI();
		}

		private void HandleTraitUpdate(string trait)
		{
			UpdateGUI();
		}

		private void UpdateGUI()
		{
			m_traitsList = m_agent.Agent.Traits.Traits.ToList();
			m_statsList = m_agent.Agent.Stats.Stats.ToList();

			m_traitListView.itemsSource = m_traitsList;
			m_statsListView.itemsSource = m_statsList;

			m_root.MarkDirtyRepaint();
		}
	}

}
