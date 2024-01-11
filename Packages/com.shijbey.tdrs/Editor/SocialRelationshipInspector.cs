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

	private VisualElement m_root;
	private List<Trait> m_traitsList;
	private List<KeyValuePair<string, Stat>> m_statsList;
	private List<StatModifier> m_statModifiers;
	private SocialRelationship m_relationship;
	private MultiColumnListView m_traitListView;
	private MultiColumnListView m_statsListView;
	private MultiColumnListView m_statModifiersView;

	public override VisualElement CreateInspectorGUI()
	{
		m_relationship = target as SocialRelationship;
		m_root = new VisualElement();
		m_UXml.CloneTree(m_root);

		CreateTraitGUI(m_root);
		CreateStatsGUI(m_root);

		return m_root;
	}

	private void CreateTraitGUI(VisualElement root)
	{
		m_traitsList = new List<Trait>();
		m_traitListView = root.Q<MultiColumnListView>(name: "Traits");


		if (m_relationship.Traits != null)
		{
			m_traitsList = m_relationship.Traits.Traits;
		}

		m_traitListView.itemsSource = m_traitsList;

		var cols = m_traitListView.columns;

		// Set makeCell

		cols["trait"].makeCell = () => new Label();
		cols["description"].makeCell = () => new TextElement() { };
		cols["cooldown"].makeCell = () => new TextElement();

		// Set bindCell

		cols["trait"].bindCell = (VisualElement e, int index) =>
		{
			(e as Label).text = m_traitsList[index].DisplayName;
		};

		cols["description"].bindCell = (VisualElement e, int index) =>
		{
			(e as TextElement).text = m_traitsList[index].Description;
		};

		cols["cooldown"].bindCell = (VisualElement e, int index) =>
		{
			if (m_traitsList[index].Duration < 0)
			{
				(e as TextElement).text = "n/a";
			}
			else
			{
				(e as TextElement).text = m_traitsList[index].Duration.ToString();
			}
		};
	}

	private void CreateStatsGUI(VisualElement root)
	{
		m_statsList = new List<KeyValuePair<string, Stat>>();
		m_statModifiers = new List<StatModifier>();

		m_statsListView = root.Q<MultiColumnListView>(name: "Stats");

		if (m_relationship.Stats != null && m_relationship.Stats.Modifiers != null)
		{
			m_statsList = m_relationship.Stats.Stats.ToList();
			m_statModifiers = m_relationship.Stats.Modifiers.ToList();
		}

		m_statsListView.itemsSource = m_statsList;

		var cols = m_statsListView.columns;

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

		m_statModifiersView = root.Q<MultiColumnListView>(name: "StatModifiers");

		m_statModifiersView.itemsSource = m_statModifiers;

		var modifierCols = m_statModifiersView.columns;

		// Set makeCell

		modifierCols["stat"].makeCell = () => new Label();
		modifierCols["value"].makeCell = () => new TextElement();
		modifierCols["description"].makeCell = () => new TextElement();
		modifierCols["cooldown"].makeCell = () => new TextElement();

		// Set bindCell

		modifierCols["stat"].bindCell = (VisualElement e, int index) =>
		{
			(e as Label).text = m_statModifiers[index].Stat;
		};

		modifierCols["value"].bindCell = (VisualElement e, int index) =>
		{
			(e as TextElement).text = $"{m_statModifiers[index].Value}";
		};

		modifierCols["description"].bindCell = (VisualElement e, int index) =>
		{
			(e as TextElement).text = m_statModifiers[index].Description;
		};

		modifierCols["cooldown"].bindCell = (VisualElement e, int index) =>
		{
			if (m_statModifiers[index].Duration < 0)
			{
				(e as TextElement).text = "n/a";
			}
			else
			{
				(e as TextElement).text = m_statModifiers[index].Duration.ToString();
			}
		};
	}

	public void OnEnable()
	{
		m_relationship = target as SocialRelationship;

		m_relationship.OnTick.AddListener(HandleOnTick);
		m_relationship.OnStatChange.AddListener(HandleOnStatChange);
		m_relationship.OnTraitAdded.AddListener(HandleTraitUpdate);
		m_relationship.OnTraitRemoved.AddListener(HandleTraitUpdate);
	}

	public void OnDisable()
	{
		m_relationship.OnTick.RemoveListener(HandleOnTick);
		m_relationship.OnStatChange.RemoveListener(HandleOnStatChange);
		m_relationship.OnTraitAdded.RemoveListener(HandleTraitUpdate);
		m_relationship.OnTraitRemoved.RemoveListener(HandleTraitUpdate);
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
		m_traitsList = m_relationship.Traits.Traits;
		m_statsList = m_relationship.Stats.Stats.ToList();
		m_statModifiers = m_relationship.Stats.Modifiers.ToList();

		m_traitListView.itemsSource = m_traitsList;
		m_statModifiersView.itemsSource = m_statModifiers;
		m_statsListView.itemsSource = m_statsList;

		m_root.MarkDirtyRepaint();
	}
}