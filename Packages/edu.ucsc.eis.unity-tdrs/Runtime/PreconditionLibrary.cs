using System.Collections.Generic;

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
