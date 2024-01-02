using System;
using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// A collection of social rules
	/// </summary>
	public class SocialRules
	{
		#region Attributes

		/// <summary>
		/// The social rules that are active and affecting relationships
		/// </summary>
		protected List<SocialRule> _rules;

		#endregion

		#region Properties

		public IEnumerable<SocialRule> Rules => _rules;

		#endregion

		#region Events

		public event EventHandler<SocialRule> OnRuleAdded;
		public event EventHandler<SocialRule> OnRuleRemoved;

		#endregion

		#region Constructors

		public SocialRules()
		{
			_rules = new List<SocialRule>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add a rule to the entities collection of active rules.
		/// </summary>
		/// <param name="rule"></param>
		public virtual void AddSocialRule(SocialRule rule)
		{
			_rules.Add(rule);
			if (OnRuleAdded != null) OnRuleAdded.Invoke(this, rule);
		}

		public virtual void RemoveSocialRule(SocialRule rule)
		{
			if (!HasSocialRule(rule))
			{
				return;
			}

			_rules.Remove(rule);
			if (OnRuleRemoved != null) OnRuleRemoved.Invoke(this, rule);
		}

		/// <summary>
		/// Removes all social rules that have the given source
		/// </summary>
		/// <param name="source"></param>
		public void RemoveAllSocialRulesFromSource(object source)
		{
			// Loop backward through the social rules and remove all that have the given source
			for (int i = _rules.Count; i > 0; i--)
			{
				var rule = _rules[i];

				if (rule.Source == source)
				{
					// Get all the entities relationships and check them for the rule
					_rules.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Check if a social rule is present
		/// </summary>
		/// <param name="rule"></param>
		/// <returns></returns>
		public bool HasSocialRule(SocialRule rule)
		{
			return _rules.Contains(rule);
		}

		#endregion
	}
}
