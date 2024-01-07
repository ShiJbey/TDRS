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
	public class SocialRuleDefinition
	{
		#region Fields

		protected DBQuery m_query;
		protected string[] m_effects;
		protected string m_descriptionTemplate;
		protected object m_source;

		#endregion

		#region Properties

		public DBQuery Query => m_query;
		public string[] Effects => m_effects;
		public string DescriptionTemplate => m_descriptionTemplate;
		public object Source => m_source;

		#endregion

		#region Constructors

		public SocialRuleDefinition()
		{
			m_query = null;
			m_effects = new string[0];
			m_descriptionTemplate = "";
			m_source = null;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Set the source for the social rule.
		/// </summary>
		/// <param name="source"></param>
		public void SetSource(object source)
		{
			m_source = source;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Create a new social rule definition from Yaml data.
		/// </summary>
		/// <param name="yamlNode"></param>
		/// <returns></returns>
		public static SocialRuleDefinition FromYaml(YamlNode yamlNode)
		{
			SocialRuleDefinition ruleDef = new SocialRuleDefinition();

			// Try to set the query
			if (yamlNode.TryGetChild("where", out var whereNode))
			{
				ruleDef.m_query = new DBQuery(
					whereNode.GetValue()
						.Split("\n")
						.Where(clause => clause != "")
						.ToArray()
				);
			}

			// Try to set the effects
			if (yamlNode.TryGetChild("apply", out var effectsNode))
			{
				ruleDef.m_effects = (effectsNode as YamlSequenceNode).Children
					.Select(node => node.GetValue())
					.ToArray();
			}
			else
			{
				throw new ArgumentException("Social rule definition is missing 'apply' section");
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
