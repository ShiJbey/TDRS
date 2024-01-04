using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace TDRS
{
	public abstract class EffectFactory : MonoBehaviour, IEffectFactory
	{
		public abstract IEffect Instantiate(SocialEngine engine, YamlNode effectNode);
	}
}
