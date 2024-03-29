﻿using System.Collections.Generic;
namespace TDRS
{
	/// <summary>
	/// A repository of all the various trait types that exist in the game.
	/// </summary>
	public class TraitLibrary
	{
		#region Properties

		/// <summary>
		/// Repository of definition data for traits
		/// </summary>
		public Dictionary<string, Trait> Traits { get; }

		#endregion

		#region Constructors

		public TraitLibrary()
		{
			Traits = new Dictionary<string, Trait>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a trait to the library
		/// </summary>
		/// <param name="trait"></param>
		public void AddTrait(Trait trait)
		{
			Traits[trait.TraitID] = trait;
		}

		#endregion
	}
}
