using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// This MonoBehaviour is meant to be attached to the same GameObject
	/// containing the TDRSManager. It subscribes to the manager's OnLoadFactories
	/// event and adds new precondition and effect factory instances to the
	/// manager's libraries
	/// </summary>
	public class DefaultTDRSFactories : MonoBehaviour
	{
		public void LoadFactories(SocialEngine engine)
		{
			engine.EffectLibrary.AddFactory("StatBuff", new StatBuffEffectFactory());
			engine.EffectLibrary.AddFactory("AddSocialRule", new AddSocialRuleFactory());
			engine.PreconditionLibrary.AddFactory("TargetHasTrait", new TargetHasTraitFactory());
			engine.PreconditionLibrary.AddFactory("OwnerHasTrait", new OwnerHasTraitFactory());
		}
	}
}
