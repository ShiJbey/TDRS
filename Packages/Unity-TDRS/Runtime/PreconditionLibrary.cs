using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace TDRS
{
	/// <summary>
	/// A repository of all the possible precondition types available in the game.
	/// </summary>
	public class PreconditionLibrary
	{
		#region Attributes

		/// <summary>
		/// Precondition IDs mapped to factories that construct that precondition.
		/// </summary>
		protected Dictionary<string, IPreconditionFactory> _factories = new Dictionary<string, IPreconditionFactory>();

		/// <summary>
		/// Precondition factories to import ito the library
		/// </summary>
		public List<PreconditionFactorySO> preconditionFactories = new List<PreconditionFactorySO>();

		#endregion

		#region Methods

		/// <summary>
		/// Add a new factory to the library.
		/// </summary>
		/// <param name="preconditionID"></param>
		/// <param name="factory"></param>
		public void AddFactory(string preconditionID, IPreconditionFactory factory)
		{
			_factories[preconditionID] = factory;
		}

		/// <summary>
		/// Create a new precondition instance.
		/// </summary>
		/// <param name="preconditionID"></param>
		/// <param name="preconditionNode"></param>
		/// <returns></returns>
		public IPrecondition CreatePrecondition(TDRSManager manager, string preconditionID, YamlNode preconditionNode)
		{
			var factory = _factories[preconditionID];

			var precondition = factory.Instantiate(manager, preconditionNode);

			return precondition;
		}

		/// <summary>
		/// Get the precondition factory mapped to a given ID.
		/// </summary>
		/// <param name="preconditionID"></param>
		/// <returns></returns>
		public IPreconditionFactory GetPreconditionFactory(string preconditionID)
		{
			return _factories[preconditionID];
		}
		#endregion
	}
}
