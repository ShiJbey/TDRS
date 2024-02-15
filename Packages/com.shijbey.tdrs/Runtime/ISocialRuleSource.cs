using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// An object that can supply social rules and manage instances of those social rules.
	/// </summary>
	public interface ISocialRuleSource
	{
		/// <summary>
		/// The collection of social rules that belong to this source.
		/// </summary>
		public IList<SocialRule> SocialRules { get; }

		/// <summary>
		/// The collection of social rule instances that belong to this source.
		/// </summary>
		public IList<SocialRuleInstance> SocialRuleInstances { get; }

		/// <summary>
		/// Add a social rule instance to the collection.
		/// </summary>
		/// <param name="instance"></param>
		public void AddSocialRuleInstance(SocialRuleInstance instance);

		/// <summary>
		/// Check if an instance of a social rule already exists for a rule.
		/// </summary>
		/// <param name="rule"></param>
		/// <param name="owner"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool HasSocialRuleInstance(SocialRule rule, string owner, string other);

		/// <summary>
		/// Get an instance of a social rule already exists for a rule.
		/// </summary>
		/// <param name="rule"></param>
		/// <param name="owner"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public SocialRuleInstance GetSocialRuleInstance(
			SocialRule rule,
			string owner,
			string other
		);

		/// <summary>
		/// Remove a social rule instance from the collection.
		/// </summary>
		/// <param name="instance"></param>
		public bool RemoveSocialRuleInstance(SocialRuleInstance instance);
	}
}
