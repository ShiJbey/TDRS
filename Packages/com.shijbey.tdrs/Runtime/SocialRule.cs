using System;
using System.Linq;
using YamlDotNet.RepresentationModel;
using RePraxis;
using TDRS.Helpers;

namespace TDRS
{
	/// <summary>
	/// Data used to instantiate social rules when instantiating traits
	/// </summary>
	public class SocialRule
	{
		#region Fields

		protected DBQuery m_query;
		protected string[] m_effects;
		protected string m_descriptionTemplate;
		protected TraitDefinition m_source;

		#endregion

		#region Properties

		public DBQuery Query => m_query;
		public string[] Effects => m_effects;
		public string DescriptionTemplate => m_descriptionTemplate;
		public TraitDefinition Source
		{
			get { return m_source; }
			set { m_source = value; }
		}

		#endregion

		#region Constructors

		public SocialRule()
		{
			m_query = null;
			m_effects = new string[0];
			m_descriptionTemplate = "";
			m_source = null;
		}

		public SocialRule(DBQuery precondition, string[] effects, string description)
		{
			m_query = precondition;
			m_effects = effects;
			m_descriptionTemplate = description;
			m_source = null;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Create a new social rule definition from Yaml data.
		/// </summary>
		/// <param name="yamlNode"></param>
		/// <returns></returns>
		public static SocialRule FromYaml(TraitDefinition traitDef, YamlNode yamlNode)
		{
			SocialRule ruleDef = new SocialRule()
			{
				m_descriptionTemplate = traitDef.DescriptionTemplate,
				m_source = traitDef
			};

			// Try to set the query
			if (yamlNode.TryGetChild("precondition", out var preconditionNode))
			{
				ruleDef.m_query = new DBQuery(
					preconditionNode.GetValue()
						.Split("\n")
						.Where(clause => clause != "")
						.ToArray()
				);
			}

			// Try to set the effects
			if (yamlNode.TryGetChild("effects", out var effectsNode))
			{
				ruleDef.m_effects = (effectsNode as YamlSequenceNode).Children
					.Select(node => node.GetValue())
					.ToArray();
			}
			else
			{
				throw new ArgumentException("Social rule definition is missing 'effects' section");
			}

			if (yamlNode.TryGetChild("description", out var descriptionNode))
			{
				ruleDef.m_descriptionTemplate = descriptionNode.GetValue();
			}

			return ruleDef;
		}

		#endregion
	}
}
