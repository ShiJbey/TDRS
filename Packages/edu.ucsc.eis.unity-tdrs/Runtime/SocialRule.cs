#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace TDRS
{
	/// <summary>
	/// Rules that are applied when a character is calculating their opinion of
	/// another. Rules have preconditions that must be met before they may be
	/// applied.
	/// </summary>
	public class SocialRule
	{
		#region Attributes

		/// <summary>
		/// Preconditions that need to pass for the social rule to be applied
		/// </summary>
		protected readonly List<IPrecondition> _preconditions;

		/// <summary>
		/// Effects applied by the social rule if its preconditions pass
		/// </summary>
		protected readonly List<IEffect> _effects;

		/// <summary>
		/// is True if this rule is applied to outgoing relationships
		/// </summary>
		protected readonly bool _isOutgoing = true;

		/// <summary>
		/// The object responsible to creating and adding this rule to a collection
		/// </summary>
		protected readonly object? _source = null;

		#endregion

		#region Properties

		/// <summary>
		/// Preconditions that need to pass for the social rule to be applied
		/// </summary>
		public IEnumerable<IPrecondition> Preconditions => _preconditions;

		/// <summary>
		/// Effects applied by the social rule if its preconditions pass
		/// </summary>
		public IEnumerable<IEffect> Effects => _effects;

		/// <summary>
		/// is True if this rule is applied to outgoing relationships
		/// </summary>
		public bool IsOutgoing => _isOutgoing;

		/// <summary>
		/// The object responsible to creating and adding this rule to a collection
		/// </summary>
		public object? Source => _source;

		#endregion

		#region Constructors

		public SocialRule(
			IEnumerable<IPrecondition> preconditions,
			IEnumerable<IEffect> effects,
			bool outgoing = true,
			object? source = null
			)
		{
			_preconditions = preconditions.ToList();
			_effects = effects.ToList();
			_isOutgoing = outgoing;
			_source = source;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Check that a given relationship passes the preconditions of the social rule
		/// </summary>
		/// <param name="relationship"></param>
		/// <returns></returns>
		public bool CheckPreconditions(SocialEntity relationship)
		{
			foreach (var precondition in _preconditions)
			{
				if (precondition.CheckPrecondition(relationship) == false)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Callback method executed when the social rule is applied to a relationship.
		/// </summary>
		/// <param name="relationship"></param>
		public void OnAdd(TDRSRelationship relationship)
		{
			foreach (var effect in _effects)
			{
				effect.Apply(relationship);
			}
		}

		/// <summary>
		/// Callback method executed when the social rule is removed from a relationship.
		/// </summary>
		/// <param name="relationship"></param>
		public void OnRemove(TDRSRelationship relationship)
		{
			foreach (var effect in _effects)
			{
				effect.Remove(relationship);
			}
		}

		#endregion
	}
}
