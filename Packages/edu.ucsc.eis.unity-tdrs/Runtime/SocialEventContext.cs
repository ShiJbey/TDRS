using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// Contains information an bindings used to instantiate effects
	/// </summary>
	public class EffectBindingContext
	{
		#region Fields

		protected string m_description;
		protected SocialEngine m_socialEngine;
		protected Dictionary<string, string> m_variableBindings;

		#endregion

		#region Properties

		public string Description => m_description;
		public SocialEngine Engine => m_socialEngine;
		public Dictionary<string, string> Bindings => m_variableBindings;

		#endregion

		public EffectBindingContext(SocialEngine socialEngine, SocialEventType eventType, params string[] agents)
		{
			m_socialEngine = socialEngine;
			m_description = eventType.DescriptionTemplate;
			m_variableBindings = new Dictionary<string, string>();


			for (int i = 0; i < eventType.Roles.Length; i++)
			{
				string role = eventType.Roles[i];
				string agentID = agents[i];
				m_variableBindings[role] = agentID;
				m_description = m_description.Replace($"[{role.Substring(1)}]", agentID);
			}

		}

		public EffectBindingContext(SocialAgent agent, string descriptionTemplate)
		{
			m_socialEngine = agent.Engine;
			m_description = descriptionTemplate;
			m_variableBindings = new Dictionary<string, string>()
			{
				{ "?owner", agent.UID }
			};

			foreach (var pair in m_variableBindings)
			{
				string variableName = pair.Key;
				string agentID = pair.Value;
				m_description = m_description.Replace($"[{variableName.Substring(1)}]", agentID);
			}
		}

		public EffectBindingContext(SocialRelationship agent, string descriptionTemplate)
		{
			m_socialEngine = agent.Engine;
			m_description = descriptionTemplate;
			m_variableBindings = new Dictionary<string, string>()
			{
				{ "?owner", agent.Owner.UID },
				{ "?target", agent.Target.UID },
			};

			foreach (var pair in m_variableBindings)
			{
				string variableName = pair.Key;
				string agentID = pair.Value;
				m_description = m_description.Replace($"[{variableName.Substring(1)}]", agentID);
			}
		}

		public EffectBindingContext(EffectBindingContext ctx)
		{
			m_socialEngine = ctx.m_socialEngine;
			m_description = ctx.m_description;
			m_variableBindings = new Dictionary<string, string>(ctx.m_variableBindings);
		}

		public EffectBindingContext WithBindings(Dictionary<string, string> bindings)
		{
			var updatedCtx = new EffectBindingContext(this);

			foreach (var pair in bindings)
			{
				updatedCtx.Bindings[pair.Key] = pair.Value;
			}

			return updatedCtx;
		}
	}
}
