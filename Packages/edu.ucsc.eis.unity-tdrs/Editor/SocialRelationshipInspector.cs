using UnityEditor;
using UnityEngine.UIElements;
using TDRS;
using System.Collections.Generic;
using TDRS.StatSystem;
using System.Linq;

[CustomEditor(typeof(SocialRelationship))]
public class SocialRelationshipInspector : Editor
{
	public VisualTreeAsset m_UXml;

	private List<Trait> m_traitsList = new List<Trait>();
	private List<KeyValuePair<string, Stat>> m_statsList;
	private List<StatModifier> m_statModifiers;
	private SocialRelationship m_relationship;

	public override VisualElement CreateInspectorGUI()
	{
		var root = new VisualElement();
		m_UXml.CloneTree(root);

		CreateTraitGUI(root);
		CreateStatsGUI(root);

		return root;
	}

	private void CreateTraitGUI(VisualElement root)
	{
		var traitListView = root.Q<MultiColumnListView>(name: "Traits");

		m_relationship = target as SocialRelationship;

		if (m_relationship.Traits != null)
		{
			m_traitsList = m_relationship.Traits.Traits;
		}

		traitListView.itemsSource = m_traitsList;

		var cols = traitListView.columns;

		// Set makeCell

		cols["trait"].makeCell = () => new Label();
		cols["description"].makeCell = () => new TextElement() { };

		// Set bindCell

		cols["trait"].bindCell = (VisualElement e, int index) =>
		{
			(e as Label).text = m_traitsList[index].DisplayName;
		};

		cols["description"].bindCell = (VisualElement e, int index) =>
		{
			(e as TextElement).text = m_traitsList[index].Description;
		};
	}

	private void CreateStatsGUI(VisualElement root)
	{
		m_statsList = new List<KeyValuePair<string, Stat>>();
		m_statModifiers = new List<StatModifier>();

		var statListView = root.Q<MultiColumnListView>(name: "Stats");

		m_relationship = target as SocialRelationship;

		if (m_relationship.Stats != null && m_relationship.Stats.Modifiers != null)
		{
			m_statsList = m_relationship.Stats.Stats.ToList();
			m_statModifiers = m_relationship.Stats.Modifiers.ToList();
		}

		statListView.itemsSource = m_statsList;

		var cols = statListView.columns;

		// Set makeCell

		cols["stat"].makeCell = () => new Label();
		cols["value"].makeCell = () => new TextElement() { };

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

		var modifiersListView = root.Q<ListView>(name: "StatModifiersList");

		modifiersListView.itemsSource = m_statModifiers;

		modifiersListView.makeItem = () => new TextElement();

		modifiersListView.bindItem = (VisualElement e, int index) =>
		{
			var modifier = m_statModifiers[index];

			if (modifier.Source != null)
			{
				(e as TextElement).text = modifier.Description;
			}
			else
			{
				(e as TextElement).text = "";
			}
		};
	}

	public override bool RequiresConstantRepaint()
	{
		return true;
	}
}
