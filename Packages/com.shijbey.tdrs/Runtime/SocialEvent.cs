using YamlDotNet.RepresentationModel;
using TDRS.Helpers;
using System.Linq;
using System.Collections.Generic;
using RePraxis;

namespace TDRS
{
	/// <summary>
	/// Definition information for creating SocialEvent instances
	/// </summary>
	public class SocialEvent
	{
		#region Fields

		protected string m_name;
		protected string[] m_roles;
		protected string m_descriptionTemplate;
		protected SocialEventResponse[] m_responses;

		#endregion

		#region Properties

		public string Name => m_name;
		public string[] Roles => m_roles;
		public string DescriptionTemplate => m_descriptionTemplate;
		public SocialEventResponse[] Responses => m_responses;
		public int Cardinality => m_roles.Length;

		#endregion

		#region Constructors

		public SocialEvent()
		{
			m_name = "";
			m_roles = new string[0];
			m_descriptionTemplate = "";
			m_responses = new SocialEventResponse[0];
		}

		public SocialEvent(
			string name,
			string[] roles,
			string description,
			SocialEventResponse[] responses
		)
		{
			m_name = name;
			m_roles = roles;
			m_descriptionTemplate = description;
			m_responses = responses;
		}

		#endregion

		#region Methods

		public override string ToString()
		{
			return $"{m_name}/{Cardinality}";
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Create a new SocialEventType instance from YAML
		/// </summary>
		/// <param name="yamlNode"></param>
		/// <returns></returns>
		public static SocialEvent FromYaml(YamlNode yamlNode)
		{
			SocialEvent eventType = new SocialEvent() { };

			// Set the event and role names

			var eventHeader = yamlNode.GetChild("event").GetValue();
			var eventHeaderParts = eventHeader.Split(" ").Select(s => s.Trim()).ToList();

			eventType.m_name = eventHeaderParts[0]; // Take the first part as the event name
			eventHeaderParts.RemoveAt(0); // Remove the event name to leave the role names
			eventType.m_roles = eventHeaderParts.ToArray(); // Set the event role names

			// Set the description template

			eventType.m_descriptionTemplate = yamlNode.GetChild("description").GetValue();

			// Set the responses

			var routesNode = (YamlSequenceNode)yamlNode.TryGetChild("responses");
			if (routesNode != null)
			{
				List<SocialEventResponse> responses = new List<SocialEventResponse>();

				foreach (YamlMappingNode routeNode in routesNode.Children)
				{
					responses.Add(SocialEventResponse.FromYaml(routeNode));
				}

				eventType.m_responses = responses.ToArray();
			}

			return eventType;
		}

		#endregion
	}

	public class SocialEventResponse
	{
		#region Fields

		protected DBQuery m_query;
		protected string[] m_effects;

		#endregion

		#region Properties

		public DBQuery Query => m_query;
		public string[] Effects => m_effects;

		#endregion

		#region Constructors

		public SocialEventResponse()
		{
			m_query = null;
			m_effects = new string[0];
		}

		public SocialEventResponse(DBQuery precondition, string[] effects)
		{
			m_query = precondition;
			m_effects = effects;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Create a SocialEventResponse instance from YAML
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static SocialEventResponse FromYaml(YamlNode node)
		{
			var response = new SocialEventResponse();

			// Try to set the query

			if (node.TryGetChild("precondition", out var preconditionNode))
			{
				response.m_query = new DBQuery(
					preconditionNode.GetValue()
						.Split("\n")
						.Where(clause => clause != "")
						.ToArray()
				);
			}

			// Try to set the effects

			if (node.TryGetChild("effects", out var effectsNode))
			{
				List<string> effects = new List<string>();

				foreach (var effect in (effectsNode as YamlSequenceNode).Children)
				{
					effects.Add(effect.GetValue());
				}

				response.m_effects = effects.ToArray();
			}

			return response;
		}

		#endregion
	}
}
