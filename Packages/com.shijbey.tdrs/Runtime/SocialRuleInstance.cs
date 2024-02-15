using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// A record of the application of a social rule. It maintains what bindings were
	/// involved in the social rule and the instances of the effects that were applied
	/// </summary>
	public class SocialRuleInstance : EffectGroup
	{
		#region Properties

		/// <summary>
		/// The rule that this is an instance of.
		/// </summary>
		public SocialRule Rule { get; }

		/// <summary>
		/// The UID of the owner of the relationship this was instanced for.
		/// </summary>
		public string Owner { get; }

		/// <summary>
		/// The UID of the other character the rule was instanced for.
		/// </summary>
		public string Other { get; }

		#endregion

		#region Constructors

		public SocialRuleInstance(
			EffectContext ctx,
			SocialRule rule,
			List<IEffect> effects
		) : base(ctx, -1)
		{
			Rule = rule;
			Owner = Context.Bindings["?owner"].ToString();
			Other = Context.Bindings["?other"].ToString();
			Effects = effects;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Attempt to create an instance of a social rule given a context.
		/// </summary>
		/// <param name="socialRule"></param>
		/// <param name="ctx"></param>
		/// <returns></returns>
		public static SocialRuleInstance Instantiate(SocialRule socialRule, EffectContext ctx)
		{
			List<IEffect> effects = new List<IEffect>();

			foreach (var entry in socialRule.Effects)
			{
				effects.Add(ctx.Engine.EffectLibrary.CreateInstance(ctx, entry));
			}

			var ruleInstance = new SocialRuleInstance(ctx, socialRule, effects);

			return ruleInstance;
		}

		#endregion
	}
}
