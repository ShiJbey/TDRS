using System;
using System.Collections.Generic;
using System.Linq;
using RePraxis;

namespace TDRS
{
	/// <summary>
	/// SocialEventDef data paired with parameters for deferred execution.
	/// </summary>
	public class SocialEventInstance
	{
		#region Fields

		private SocialEngine m_engine;
		private SocialEventType m_eventType;
		private EffectContext m_baseContext;

		#endregion

		#region Properties

		public SocialEngine Engine => m_engine;
		public SocialEventType EventType => m_eventType;
		public string Description => m_baseContext.Description;
		public Dictionary<string, object> Bindings => m_baseContext.Bindings;

		#endregion

		#region Constructors

		public SocialEventInstance(
			SocialEngine engine,
			SocialEventType eventType,
			params string[] agents
		)
		{
			m_engine = engine;
			m_eventType = eventType;

			var bindings = new Dictionary<string, object>();
			for (int i = 0; i < eventType.Roles.Length; i++)
			{
				string role = eventType.Roles[i];
				string agentID = agents[i];
				bindings[role] = agentID;
			}

			// Create the base context for the events
			m_baseContext = new EffectContext(engine, eventType.Description, bindings);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Execute the effects of this event and dispatch a C# event.
		/// </summary>
		public void Dispatch()
		{
			m_engine.DispatchEvent(this);
		}

		/// <summary>
		/// Execute the effects of the event on the social engine.
		/// </summary>
		public void Execute()
		{
			foreach (var response in m_eventType.Responses)
			{
				var results = new DBQuery(response.Preconditions)
					.Run(m_baseContext.Engine.DB, m_baseContext.Bindings);

				// Skip this response because the query failed
				if (!results.Success) continue;

				// Create a new context for each binding result
				foreach (var bindingSet in results.Bindings)
				{
					var scopedCtx = m_baseContext.WithBindings(bindingSet);

					if (response.Description != "")
					{
						scopedCtx.DescriptionTemplate = response.Description;
					}

					try
					{
						var effects = response.Effects
							.Select(effectString =>
							{
								return m_engine.EffectLibrary.CreateInstance(scopedCtx, effectString);
							});

						foreach (var effect in effects)
						{
							effect.Apply();
						}
					}
					catch (ArgumentException ex)
					{
						throw new ArgumentException(
							$"Error encountered while instantiating effects for '{m_eventType.Name}' event: "
							+ ex.Message
						);
					}
				}
			}
		}

		#endregion
	}
}
