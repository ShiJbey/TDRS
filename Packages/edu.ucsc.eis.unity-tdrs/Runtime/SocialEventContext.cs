using System.Collections.Generic;

namespace TDRS
{
	public class SocialEventContext
	{
		protected string m_description;
		protected SocialEngine m_socialEngine;
		protected Dictionary<string, string> m_variableBindings;

		public string Description => m_description;
		public SocialEngine Engine => m_socialEngine;
		public Dictionary<string, string> Bindings => m_variableBindings;

		public SocialEventContext(SocialEngine socialEngine, SocialEventType eventType, params string[] agents)
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

		public SocialEventContext(SocialEventContext ctx)
		{
			m_socialEngine = ctx.m_socialEngine;
			m_description = ctx.m_description;
			m_variableBindings = ctx.m_variableBindings;
		}

		public SocialEventContext WithBindings(Dictionary<string, string> bindings)
		{
			var updatedCtx = new SocialEventContext(this);

			foreach (var pair in bindings)
			{
				updatedCtx.Bindings[pair.Key] = pair.Value;
			}

			return updatedCtx;
		}
	}
}
