using System;
using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// A collection of social rules
	/// </summary>
	public class SocialRuleManager
	{
		#region Fields

		/// <summary>
		/// Social rules to check when a character gains a new incoming or outgoing relationship.
		/// </summary>
		protected List<SocialRuleDefinition> m_rules;

		protected List<SocialRuleInstance> m_ruleInstances;

		#endregion

		#region Properties

		public IList<SocialRuleDefinition> Rules => m_rules;
		public IList<SocialRuleInstance> SocialRuleInstances => m_ruleInstances;

		#endregion

		#region Events

		public event EventHandler<SocialRuleDefinition> OnRuleAdded;
		public event EventHandler<SocialRuleDefinition> OnRuleRemoved;

		#endregion

		#region Constructors

		public SocialRuleManager()
		{
			m_rules = new List<SocialRuleDefinition>();
			m_ruleInstances = new List<SocialRuleInstance>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a rule to the entities collection of active rules.
		/// </summary>
		/// <param name="rule"></param>
		public virtual void AddSocialRuleDefinition(SocialRuleDefinition rule)
		{
			m_rules.Add(rule);
			if (OnRuleAdded != null) OnRuleAdded.Invoke(this, rule);
		}

		public void RemoveSocialRuleDefinition(SocialRuleDefinition rule)
		{
			if (m_rules.Remove(rule))
			{
				RemoveAllInstancesOfRule(rule);
				if (OnRuleRemoved != null) OnRuleRemoved.Invoke(this, rule);
			}
		}

		public bool HasSocialRuleInstance(SocialRuleDefinition rule, string owner, string other)
		{
			foreach (var instance in m_ruleInstances)
			{
				if (instance.Source == rule && instance.Owner == owner && instance.Other == other)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Add a social rule instance to the collection
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="source"></param>
		public void AddSocialRuleInstance(SocialRuleInstance instance)
		{
			m_ruleInstances.Add(instance);
			instance.Apply();
		}

		/// <summary>
		/// Remove all social rule instances from a given source
		/// </summary>
		/// <param name="source"></param>
		private void RemoveAllInstancesOfRule(SocialRuleDefinition rule)
		{
			// Loop backward through the social rules instances and remove all that have the
			// given source
			for (int i = m_ruleInstances.Count - 1; i > 0; i--)
			{
				var ruleInstance = m_ruleInstances[i];

				if (ruleInstance.Source == rule)
				{
					// Remove the rule
					m_ruleInstances.RemoveAt(i);

					// Undo all its effects
					ruleInstance.Remove();
				}
			}
		}

		/// <summary>
		/// Removes all social rules that have the given source
		/// </summary>
		/// <param name="source"></param>
		public void RemoveAllSocialRulesFromSource(object source)
		{
			// Loop backward through the social rules and remove all that have the given source
			for (int i = m_rules.Count - 1; i > 0; i--)
			{
				var rule = m_rules[i];

				if (rule.Source == source)
				{
					// Get all the entities relationships and check them for the rule
					RemoveSocialRuleDefinition(rule);
				}
			}
		}

		#endregion
	}
}
