using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace TDRS
{
	/// <summary>
	/// EffectFactory Scriptable Objects provide a way to configure EffectFactories
	/// within the Unity inspector.
	///
	/// <para>
	/// Developers need to create new C# classes that inherit from EffectFactorySO to create
	/// new factories. Also, they should add a [CreateAssetMenu()] directive above the class
	/// definition to facilitate instantiating the scriptable object within Unity's editor.
	/// </para>
	/// </summary>
	public abstract class EffectFactorySO : ScriptableObject, IEffectFactory
	{
		/// <summary>
		/// The ID mapped to this factory in the Library. This is should usually be
		/// set to the class name of the constructed effect, but this is left to the discretion
		/// of the game developer.
		/// </summary>
		public string effectType = "";

		public abstract IEffect Instantiate(TDRSManager manager, YamlNode effectNode);
	}
}
