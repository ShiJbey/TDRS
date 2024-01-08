using System.Collections.Generic;
using YamlDotNet.RepresentationModel;
using TDRS.Helpers;
using System;
using System.Linq;

namespace TDRS
{
	/// <summary>
	/// Data used to instantiate traits
	/// </summary>
	public class TraitDefinition
	{
		#region Fields

		protected string m_traitID;
		protected TraitType m_traitType;
		protected string m_displayName;
		protected string m_descriptionTemplate;
		protected string[] m_effects;
		protected SocialRuleDefinition[] m_socialRules;
		protected HashSet<string> m_conflictingTraits;
		protected int m_duration;

		#endregion

		#region Properties

		/// <summary>
		/// The unique ID of the trait
		/// </summary>
		public string TraitID => m_traitID;

		/// <summary>
		/// The name of the trait as displayed in GUIs
		/// </summary>
		public string DisplayName => m_displayName;

		/// <summary>
		/// The type of object this trait is applied to
		/// </summary>
		public TraitType TraitType => m_traitType;

		/// <summary>
		/// A short textual description of the trait
		/// </summary>
		public string DescriptionTemplate => m_descriptionTemplate;

		/// <summary>
		/// Configuration data for effects associated for this trait
		/// </summary>
		public string[] Effects => m_effects;

		/// <summary>
		/// IDs of traits that this trait cannot be added with
		/// </summary>
		public HashSet<string> ConflictingTraits => m_conflictingTraits;

		/// <summary>
		/// How long the trait lasts (-1 is indefinite)
		/// </summary>
		public int Duration => m_duration;

		/// <summary>
		/// Social rules associated with this trait
		/// </summary>
		public SocialRuleDefinition[] SocialRules => m_socialRules;

		#endregion

		#region Constructors
		private TraitDefinition()
		{
			m_traitID = "";
			m_traitType = TraitType.Agent;
			m_displayName = "";
			m_descriptionTemplate = "";
			m_effects = new string[0];
			m_socialRules = new SocialRuleDefinition[0];
			m_conflictingTraits = new HashSet<string>();
			m_duration = -1;
		}

		public TraitDefinition(
			string traitID,
			TraitType traitType,
			string displayName,
			string descriptionTemplate,
			string[] effects,
			SocialRuleDefinition[] socialRules,
			HashSet<string> conflictingTraits,
			int duration = -1
		)
		{
			m_traitID = traitID;
			m_traitType = traitType;
			m_displayName = displayName;
			m_descriptionTemplate = descriptionTemplate;
			m_effects = effects;
			m_socialRules = socialRules;
			m_conflictingTraits = conflictingTraits;
			m_duration = duration;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Create a new trait definition from the given yamlNode
		/// </summary>
		/// <param name="yamlNode"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static TraitDefinition FromYaml(YamlNode yamlNode)
		{
			TraitDefinition traitDef = new TraitDefinition()
			{
				m_traitID = yamlNode.GetChild("traitID").GetValue(),
				m_traitType = TraitType.Parse<TraitType>(
					yamlNode.GetChild("traitType").GetValue(),
					true),
				m_displayName = yamlNode.GetChild("displayName").GetValue(),
				m_descriptionTemplate = yamlNode.GetChild("description").GetValue()
			};

			// Attempt to set the effects
			if (yamlNode.TryGetChild("effects", out var effectsNode))
			{
				traitDef.m_effects = (effectsNode as YamlSequenceNode).Children
					.Select(node => node.GetValue())
					.ToArray();
			}

			// Attempt to set social rules
			if (yamlNode.TryGetChild("socialRules", out var socialRulesNode))
			{
				traitDef.m_socialRules = (socialRulesNode as YamlSequenceNode).Children
					.Select(node => SocialRuleDefinition.FromYaml(traitDef, node))
					.ToArray();
			}

			// Attempt to set conflicting traits
			if (yamlNode.TryGetChild("conflictingTraits", out var conflictingTraitsNode))
			{
				traitDef.m_conflictingTraits = new HashSet<string>(
					(conflictingTraitsNode as YamlSequenceNode).Children
						.Select(node => node.GetValue())
				);
			}

			// Attempt to set duration
			if (yamlNode.TryGetChild("duration", out var durationNode))
			{
				if (int.TryParse(durationNode.GetValue(), out var duration))
				{
					traitDef.m_duration = duration;
				}
				else
				{
					throw new ArgumentException(
						$"Parameter 'duration' of trait '{traitDef.m_traitID}' must be integer"
					);
				}
			}

			return traitDef;
		}

		#endregion
	}
}
