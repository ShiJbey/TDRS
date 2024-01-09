using UnityEngine;

namespace TDRS
{
	public abstract class EffectFactory : MonoBehaviour, IEffectFactory
	{
		public abstract string EffectName { get; }
		public abstract IEffect CreateInstance(EffectBindingContext ctx, params string[] args);
	}
}
