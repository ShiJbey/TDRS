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
		public void LoadFactories(TDRSManager manager)
		{
			manager.EffectLibrary.AddFactory("StatBuff", new StatBuffEffectFactory());
			manager.EffectLibrary.AddFactory("AddSocialRule", new AddSocialRuleFactory());
			manager.PreconditionLibrary.AddFactory("TargetHasTrait", new TargetHasTraitFactory());
			manager.PreconditionLibrary.AddFactory("OwnerHasTrait", new OwnerHasTraitFactory());
		}
	}
}
