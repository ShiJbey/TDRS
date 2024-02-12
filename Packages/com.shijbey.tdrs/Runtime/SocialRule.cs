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
		protected object m_source;

		#endregion

		#region Properties

		public DBQuery Query => m_query;
		public string[] Effects => m_effects;
		public string DescriptionTemplate => m_descriptionTemplate;
		public object Source
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
	}
}
