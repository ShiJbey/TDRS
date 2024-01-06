using UnityEngine;

namespace TDRS
{
	public abstract class SocialEventEffectFactory : MonoBehaviour, ISocialEventEffectFactory
	{
		public abstract string EffectType { get; }
		public abstract ISocialEventEffect CreateInstance(SocialEventContext ctx, params string[] args);
	}
}
