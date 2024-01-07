using UnityEngine;

namespace TDRS
{
	public abstract class SocialEventEffectFactory : MonoBehaviour, ISocialEventEffectFactory
	{
		public abstract string EffectName { get; }
		public abstract ISocialEventEffect CreateInstance(EffectBindingContext ctx, params string[] args);
	}
}
