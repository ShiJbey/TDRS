using System;
using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// A record of the application of a social rule definition. It maintains what bindings were
	/// involved in the social rule and the instances of the effects that were applied
	/// </summary>
	public class SocialRuleInstance
	{
		#region Fields

		protected Dictionary<string, string> m_bindings;
		protected IEffect[] m_effects;
		protected SocialRule m_source;
		protected string m_owner;
		protected string m_other;

		#endregion

		#region Properties

		/// <summary>
		/// The bindings used to instantiate the associated effects.
		/// </summary>
		public Dictionary<string, string> Bindings => m_bindings;
		/// <summary>
		/// The instantiated effects created from the bindings.
		/// </summary>
		public IEffect[] Effects => m_effects;
		/// <summary>
		/// The social rule responsible for this instance.
		/// </summary>
		public SocialRule Source => m_source;
		/// <summary>
		/// The UID of the owner of the relationship this was instanced for.
		/// </summary>
		public string Owner => m_owner;
		/// <summary>
		/// The UID of the other character the rule was instanced for.
		/// </summary>
		public string Other => m_other;

		#endregion

		#region Constructors

		public SocialRuleInstance(
			Dictionary<string, string> bindings,
			IEffect[] effects,
			SocialRule source
		)
		{
			m_bindings = bindings;
			m_effects = effects;
			m_source = source;
			m_owner = m_bindings["?owner"];
			m_other = m_bindings["?other"];
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Apply the effects associated with the rule instance
		/// </summary>
		public void Apply()
		{
			foreach (var effect in m_effects)
			{
				effect.Apply();
			}
		}

		/// <summary>
		/// Undo the effects associated with the rule instance
		/// </summary>
		public void Remove()
		{
			foreach (var effect in m_effects)
			{
				effect.Remove();
			}
		}

		#endregion

		#region Static Methods

		public static SocialRuleInstance TryInstantiateRule(
			SocialRule socialRule,
			EffectBindingContext ctx
		)
		{
			List<IEffect> effects = new List<IEffect>();
			try
			{
				// Create instances of each of the effects associated with this rule
				foreach (var effectString in socialRule.Effects)
				{
					var effect = ctx.State.EffectLibrary.CreateInstance(ctx, effectString);
					effects.Add(effect);
				}
			}
			catch (ArgumentException)
			{
				return null;
			}

			var ruleInstance = new SocialRuleInstance(
				ctx.Bindings, effects.ToArray(), socialRule
			);

			return ruleInstance;
		}

		#endregion
	}
}
