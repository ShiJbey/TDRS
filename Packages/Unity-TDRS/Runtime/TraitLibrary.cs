#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.IO;
using YamlDotNet.RepresentationModel;
using UnityEngine;

using TDRS.Helpers;

namespace TDRS
{
	/// <summary>
	/// A repository of all the various trait types that exist in the game.
	/// </summary>
	public class TraitLibrary
	{
		#region Attributes
		/// <summary>
		/// Repository of trait IDs mapped to Trait instances
		/// </summary>
		protected Dictionary<string, Trait> _traits;

		/// <summary>
		/// Repository of definition data for traits
		/// </summary>
		protected Dictionary<string, TraitDefinition> _traitDefinitions;
		#endregion

		#region Properties
		/// <summary>
		/// Get Enumerable with all the traits in the library
		/// </summary>
		public IEnumerable<Trait> Traits => _traits.Values.ToList();
		#endregion

		#region Constructor
		public TraitLibrary()
		{
			_traits = new Dictionary<string, Trait>();
			_traitDefinitions = new Dictionary<string, TraitDefinition>();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Add a trait to the library
		///
		/// <para>
		/// This method might overwrite and existing trait if they have the same TraitID
		/// </para>
		/// </summary>
		/// <param name="trait"></param>
		public void AddTrait(Trait trait)
		{
			_traits[trait.TraitID] = trait;
		}


		/// <summary>
		/// Retrieves an existing Trait using its name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Trait GetTrait(string name)
		{
			return _traits[name];
		}

		/// <summary>
		/// Add a trait definition
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="traitID"></param>
		/// <param name="traitNode"></param>
		/// <returns></returns>
		private void AddTraitDefinition(string traitID, YamlNode traitNode)
		{
			var mapping = (YamlMappingNode)traitNode;

			// Default trait parameters

			string displayName = ((YamlScalarNode)mapping.GetChild("display_name")).GetValue();
			string description = ((YamlScalarNode)mapping.GetChild("description")).GetValue();
			HashSet<string> conflictingTraits = new HashSet<string>();

			// Get effects

			YamlNode effectsNode;
			mapping.Children.TryGetValue(new YamlScalarNode("effects"), out effectsNode);

			// Get conflicting trait IDs

			YamlNode conflictingTraitsNode;
			mapping.Children.TryGetValue(new YamlScalarNode("conflicting_traits"), out conflictingTraitsNode);
			if (conflictingTraitsNode != null)
			{
				var sequence = (YamlSequenceNode)conflictingTraitsNode;

				foreach (var entry in sequence)
				{
					string? conflictingTraitID = ((YamlScalarNode)entry).Value;
					if (conflictingTraitID != null)
					{
						conflictingTraits.Add(conflictingTraitID);
					}
				}
			}

			_traitDefinitions[traitID] = new TraitDefinition(
				traitID,
				displayName,
				description,
				effectsNode,
				conflictingTraits
			);
		}

		/// <summary>
		/// Load trait definition data from a string
		/// </summary>
		/// <param name="dataString"></param>
		public void LoadTraits(string dataString)
		{
			var input = new StringReader(dataString);

			var yaml = new YamlStream();
			yaml.Load(input);

			var rootMapping = (YamlMappingNode)yaml.Documents[0].RootNode;

			foreach (var traitEntry in rootMapping.Children)
			{
				string traitID = ((YamlScalarNode)traitEntry.Key).GetValue();

				AddTraitDefinition(traitID, traitEntry.Value);
			}
		}

		/// <summary>
		/// Instantiate all the traits within the traits definition dictionary
		/// </summary>
		/// <param name="manager"></param>
		/// <returns></returns>
		public void InstantiateTraits(TDRSManager manager)
		{
			foreach (var (traitID, traitDef) in _traitDefinitions)
			{
				// Default trait parameters

				List<IEffect> effects = new List<IEffect>();
				HashSet<string> conflictingTraits = new HashSet<string>();

				// Get effects

				YamlNode effectsNode = traitDef.EffectData;
				if (effectsNode != null)
				{
					var sequence = (YamlSequenceNode)effectsNode;

					foreach (var entry in sequence)
					{
						var effectType = entry.GetChild("type").GetValue();
						var factory = manager.EffectLibrary.GetEffectFactory(effectType);
						var effect = factory.Instantiate(manager, entry);
						effects.Add(effect);
					}
				}

				AddTrait(
					new Trait(
						traitID,
						traitDef.DisplayName,
						traitDef.Description,
						effects,
						traitDef.ConflictingTraits
					)
				);

				Debug.Log($"Instantiated '{traitID}' trait.");
			}
		}

		#endregion
	}
}
