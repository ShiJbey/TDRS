using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDRS
{
	public class AddSocialRule : IEffect
	{
		protected List<IPrecondition> _preconditions;
		protected List<IEffect> _effects;
		protected bool _isOutgoing;

		protected SocialRule _socialRule;

		public AddSocialRule(
			IEnumerable<IPrecondition> preconditions,
			IEnumerable<IEffect> effects,
			bool isOutgoing
		)
		{
			_preconditions = preconditions.ToList();
			_effects = effects.ToList();
			_isOutgoing = isOutgoing;
			_socialRule = new SocialRule(_preconditions, _effects, _isOutgoing, this);
		}

		public string Description
		{
			get
			{
				var output = new StringBuilder();

				output.Append("(Social Rule): ");

				if (_preconditions.Count > 0)
				{
					output.Append("if ");
					output.Append(
						string.Join(" and ", _preconditions.Select(p => p.Description))
					);
					output.Append(", then ");
				}

				output.Append(
					string.Join(" and ", _effects.Select(e => e.Description))
				);

				return output.ToString();
			}
		}

		public void Apply(SocialEntity target)
		{
			target.SocialRules.AddSocialRule(_socialRule);
		}

		public void Remove(SocialEntity target)
		{
			target.SocialRules.RemoveAllSocialRulesFromSource(this);
		}
	}
}
