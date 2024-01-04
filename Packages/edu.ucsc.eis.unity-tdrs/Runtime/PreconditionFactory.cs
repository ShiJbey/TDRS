using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace TDRS
{
	public abstract class PreconditionFactory : MonoBehaviour, IPreconditionFactory
	{
		public abstract IPrecondition Instantiate(SocialEngine engine, YamlNode preconditionNode);
	}
}
