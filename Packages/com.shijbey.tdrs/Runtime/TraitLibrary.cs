using System.Collections.Generic;
using System.Linq;
using System.IO;
using YamlDotNet.RepresentationModel;
using System;
using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// A repository of all the various trait types that exist in the game.
	/// </summary>
	public class TraitLibrary : MonoBehaviour
	{
		#region Fields

		/// <summary>
		/// A list of TextAssets assigned within the Unity inspector
		/// </summary>
		[SerializeField]
		protected List<TextAsset> m_definitionFiles;

		/// <summary>
		/// Repository of definition data for traits
		/// </summary>
		protected Dictionary<string, TraitDefinition> m_traitDefinitions;

		#endregion

		#region Unity Messages

		private void Awake()
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
		/// Load trait definitions from definition filed provided in the inspector
		/// </summary>
		public void LoadTraitDefinitions()
		{
			foreach (var textAsset in m_definitionFiles)
			{
				var input = new StringReader(textAsset.text);

				var yaml = new YamlStream();
				yaml.Load(input);

				var rootMapping = (YamlSequenceNode)yaml.Documents[0].RootNode;

				foreach (var node in rootMapping.Children)
				{
					AddTraitDefinition(TraitDefinition.FromYaml(node));
				}
			}
		}

		/// <summary>
		/// Instantiate all the traits within the traits definition dictionary
		/// </summary>
		/// <param name="traitID"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public Trait CreateInstance(string traitID, SocialEntity target)
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

			EffectBindingContext ctx;

			if (
				traitDefinition.TraitType == TraitType.Agent
				&& target is SocialAgent
			)
			{
				ctx = new EffectBindingContext(
					target as SocialAgent,
					traitDefinition.DescriptionTemplate
				);
			}
			else if (
				traitDefinition.TraitType == TraitType.Relationship
				&& target is SocialRelationship
			)
			{
				ctx = new EffectBindingContext(
					target as SocialRelationship,
					traitDefinition.DescriptionTemplate
				);
			}
			else
			{
				throw new ArgumentException(
					$"Trait ({traitID}) and target ({target.name}) are not of same type."
				);
			}

			// Instantiate the effects
			List<IEffect> effects = new List<IEffect>();
			foreach (var effectEntry in traitDefinition.Effects)
			{
				try
				{
					var effect = ctx.Engine.EffectFactories.CreateInstance(ctx, effectEntry);
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
