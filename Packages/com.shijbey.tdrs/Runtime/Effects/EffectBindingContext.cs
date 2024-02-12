using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// Contains information and bindings used to instantiate effects
	/// </summary>
	public class EffectBindingContext
	{
		#region Fields

		protected string m_description;
		protected SocialEngine m_socialEngine;
		protected Dictionary<string, string> m_variableBindings;

		#endregion

		#region Properties

		/// <summary>
		/// A text description of the effect
		/// </summary>
		public string Description => m_description;

		/// <summary>
		/// A reference to the game's social engine
		/// </summary>
		public SocialEngine Engine => m_socialEngine;

		/// <summary>
		/// Variable names mapped to agent ID's
		/// </summary>
		public Dictionary<string, string> Bindings => m_variableBindings;

		#endregion

		#region Constructors

		public EffectBindingContext(SocialEngine socialEngine, SocialEvent eventType, params string[] agents)
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

		public EffectBindingContext(Agent agent, string descriptionTemplate)
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

		public EffectBindingContext(Relationship relationship, string descriptionTemplate)
		{
			m_socialEngine = relationship.Engine;
			m_description = descriptionTemplate;
			m_variableBindings = new Dictionary<string, string>()
			{
				{ "?owner", relationship.Owner.UID },
				{ "?target", relationship.Target.UID },
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

		public EffectBindingContext(
			SocialEngine engine,
			string descriptionTemplate,
			Dictionary<string, string> bindings
		)
		{
			m_socialEngine = engine;
			m_description = descriptionTemplate;
			m_variableBindings = new Dictionary<string, string>(bindings);

			foreach (var pair in bindings)
			{
				string variableName = pair.Key;
				string agentID = pair.Value;
				m_description = m_description.Replace($"[{variableName.Substring(1)}]", agentID);
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Create a copy of the current context and overwrite it's bindings.
		/// </summary>
		/// <param name="bindings"></param>
		/// <returns></returns>
		public EffectBindingContext WithBindings(Dictionary<string, string> bindings)
		{
			var updatedCtx = new EffectBindingContext(this);

			foreach (var pair in bindings)
			{
				updatedCtx.Bindings[pair.Key] = pair.Value;
			}

			return updatedCtx;
		}

		#endregion
	}
}
