using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace TDRS
{
	/// <summary>
	/// PreconditionFactory Scriptable Objects provide a way to configure PreconditionFactories
	/// within the Unity inspector.
	///
	/// <para>
	/// Developers need to create new C# classes that inherit from PreconditionFactorySO to create
	/// new factories. Also, they should add a [CreateAssetMenu] directive above the class
	/// definition to facilitate instantiating the scriptable object within Unity's editor.
	/// </para>
	/// </summary>
	public abstract class PreconditionFactorySO : ScriptableObject, IPreconditionFactory
	{
		/// <summary>
		/// The ID mapped to this factory in the Library. This is should usually be
		/// set to the class name of the constructed effect, but this is left to the discretion
		/// of the game developer.
		/// </summary>
		public string preconditionType = "";

		public abstract IPrecondition Instantiate(TDRSManager manager, YamlNode preconditionNode);
	}
}
