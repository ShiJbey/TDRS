using System.Collections.Generic;
using System;

namespace TDRS
{
	/// <summary>
	/// A repository of all the various trait types that exist in the game.
	/// </summary>
	public class TraitLibrary
	{
		#region Fields

		/// <summary>
		/// Repository of definition data for traits
		/// </summary>
		protected Dictionary<string, TraitDefinition> m_traitDefinitions;

		#endregion

		#region Constructors

		public TraitLibrary()
		{
			m_traitDefinitions = new Dictionary<string, TraitDefinition>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Retrieves an existing trait definition using the trait ID
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public TraitDefinition GetTraitDefinition(string traitID)
		{
			return m_traitDefinitions[traitID];
		}

		/// <summary>
		/// Add a trait definition
		/// </summary>
		/// <param name="traitDefinition"></param>
		/// <returns></returns>
		public void AddTraitDefinition(TraitDefinition traitDefinition)
		{
			m_traitDefinitions[traitDefinition.TraitID] = traitDefinition;
		}

		/// <summary>
		/// Check if the library has a trait definition
		/// </summary>
		/// <param name="traitID"></param>
		/// /// <returns></returns>
		public bool HasTraitDefinition(string traitID)
		{
			return m_traitDefinitions.ContainsKey(traitID);
		}

		/// <summary>
		/// Instantiate all the traits within the traits definition dictionary
		/// </summary>
		/// <param name="traitID"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public Trait CreateInstance(string traitID, AgentNode target)
		{
			// Check that a definition even exists for this trait
			if (!HasTraitDefinition(traitID))
			{
				throw new KeyNotFoundException(
					$"TraitDefinition not found with ID: {traitID}"
				);
			}

			var traitDefinition = GetTraitDefinition(traitID);

			// Create a new binding context to create the effects

			if (traitDefinition.TraitType != TraitType.Agent)
			{
				throw new ArgumentException(
					$"Trait ({traitID}) and target  are not of same type."
				);
			}

			EffectBindingContext ctx = new EffectBindingContext(
				target,
				traitDefinition.DescriptionTemplate
			);


			// Instantiate the effects
			List<IEffect> effects = new List<IEffect>();
			foreach (var effectEntry in traitDefinition.Effects)
			{
				try
				{
					var effect = ctx.State.EffectLibrary.CreateInstance(ctx, effectEntry);
					effects.Add(effect);
				}
				catch (ArgumentException ex)
				{
					throw new ArgumentException(
						$"Error encountered while instantiating effects for '{traitID}' trait: "
						+ ex.Message
					);
				}
			}

			return new Trait(traitDefinition, effects.ToArray(), ctx.Description);
		}


		/// <summary>
		/// Instantiate all the traits within the traits definition dictionary
		/// </summary>
		/// <param name="traitID"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public Trait CreateInstance(string traitID, RelationshipEdge target)
		{
			// Check that a definition even exists for this trait
			if (!HasTraitDefinition(traitID))
			{
				throw new KeyNotFoundException(
					$"TraitDefinition not found with ID: {traitID}"
				);
			}

			var traitDefinition = GetTraitDefinition(traitID);

			// Create a new binding context to create the effects

			if (traitDefinition.TraitType != TraitType.Relationship)
			{
				throw new ArgumentException(
					$"Trait ({traitID}) and target are not of same type."
				);
			}

			EffectBindingContext ctx = new EffectBindingContext(
				target,
				traitDefinition.DescriptionTemplate
			);

			// Instantiate the effects
			List<IEffect> effects = new List<IEffect>();
			foreach (var effectEntry in traitDefinition.Effects)
			{
				try
				{
					var effect = ctx.State.EffectLibrary.CreateInstance(ctx, effectEntry);
					effects.Add(effect);
				}
				catch (ArgumentException ex)
				{
					throw new ArgumentException(
						$"Error encountered while instantiating effects for '{traitID}' trait: "
						+ ex.Message
					);
				}
			}

			return new Trait(traitDefinition, effects.ToArray(), ctx.Description);
		}

		#endregion
	}
}
