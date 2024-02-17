using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;


namespace TDRS
{
	[CustomEditor(typeof(RelationshipController))]
	public class RelationshipControllerEditor : Editor
	{
		public VisualTreeAsset m_UXml;

		private VisualElement m_root;
		private List<TraitInstance> m_traitsList;
		private List<KeyValuePair<string, Stat>> m_statsList;
		private List<ActiveSocialRuleEntry> m_activeSocialRuleList;
		private RelationshipController m_relationship;
		private MultiColumnListView m_traitListView;
		private MultiColumnListView m_statsListView;
		private MultiColumnListView m_activeSocialRulesView;

		public override VisualElement CreateInspectorGUI()
		{
			m_relationship = target as RelationshipController;
			m_root = new VisualElement();
			m_UXml.CloneTree(m_root);

			CreateTraitGUI();
			CreateStatsGUI();
			CreateActiveSocialRulesGUI();

			return m_root;
		}

		private void CreateTraitGUI()
		{
			m_traitsList = new List<TraitInstance>();
			m_traitListView = m_root.Q<MultiColumnListView>(name: "Traits");


			if (m_relationship.Edge != null)
			{
				m_traitsList = m_relationship.Edge.Traits.Traits.ToList();
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

		private void CreateStatsGUI()
		{
			m_statsList = new List<KeyValuePair<string, Stat>>();

			m_statsListView = m_root.Q<MultiColumnListView>(name: "Stats");

			if (m_relationship.Edge != null)
			{
				m_statsList = m_relationship.Edge.Stats.Stats.ToList();
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

		private void CreateActiveSocialRulesGUI()
		{
			m_activeSocialRuleList = new List<ActiveSocialRuleEntry>();
			m_activeSocialRulesView = m_root.Q<MultiColumnListView>(name: "ActiveSocialRules");


			if (m_relationship.Edge != null)
			{
				m_activeSocialRuleList = m_relationship.Edge.ActiveSocialRules.ToList();
			}

			m_activeSocialRulesView.itemsSource = m_activeSocialRuleList;

			var cols = m_activeSocialRulesView.columns;

			// Set makeCell

			cols["description"].makeCell = () => new TextElement();
			cols["modifiers"].makeCell = () => new TextElement();

			// Set bindCell

			cols["description"].bindCell = (VisualElement e, int index) =>
			{
				(e as TextElement).text = m_activeSocialRuleList[index].Description;
			};

			cols["modifiers"].bindCell = (VisualElement e, int index) =>
			{
				List<string> modifierDescriptions = new List<string>();

				var socialRuleEntry = m_activeSocialRuleList[index];
				foreach (var entry in socialRuleEntry.Rule.Modifiers)
				{
					modifierDescriptions.Add(
						$"{entry.StatName}: {entry.Value} ({entry.ModifierType})"
					);
				}

				(e as TextElement).text = string.Join("\n", modifierDescriptions);
			};
		}

		public void OnEnable()
		{
			m_relationship = target as RelationshipController;

			m_relationship.OnTick.AddListener(HandleOnTick);
			m_relationship.OnStatChange.AddListener(HandleOnStatChange);
			m_relationship.OnTraitAdded.AddListener(HandleTraitUpdate);
			m_relationship.OnTraitRemoved.AddListener(HandleTraitUpdate);
			m_relationship.OnRegistered.AddListener(HandleOnRegistered);

		}

		public void OnDisable()
		{
			m_relationship.OnTick.RemoveListener(HandleOnTick);
			m_relationship.OnStatChange.RemoveListener(HandleOnStatChange);
			m_relationship.OnTraitAdded.RemoveListener(HandleTraitUpdate);
			m_relationship.OnTraitRemoved.RemoveListener(HandleTraitUpdate);
			m_relationship.OnRegistered.RemoveListener(HandleOnRegistered);
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
			m_traitsList = m_relationship.Edge.Traits.Traits.ToList();
			m_statsList = m_relationship.Edge.Stats.Stats.ToList();
			m_activeSocialRuleList = m_relationship.Edge.ActiveSocialRules.ToList();

			m_traitListView.itemsSource = m_traitsList;
			m_statsListView.itemsSource = m_statsList;
			m_activeSocialRulesView.itemsSource = m_activeSocialRuleList;

			m_root.MarkDirtyRepaint();
		}
	}

}
