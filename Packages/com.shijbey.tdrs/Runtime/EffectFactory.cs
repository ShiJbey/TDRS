using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// An abstract base class for effect factories implemented as MonoBehaviors
	/// </summary>
	public abstract class EffectFactory : MonoBehaviour, IEffectFactory
	{
		public abstract string EffectName { get; }
		public abstract IEffect CreateInstance(EffectBindingContext ctx, params string[] args);
	}
}
