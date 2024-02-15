using System;
using System.Collections.Generic;
using UnityEditor;

namespace TDRS
{
	/// <summary>
	/// A record of a trait that has been applied to an agent or relationship.
	/// </summary>
	public class TraitInstance : ISocialRuleSource
	{
		#region Fields

		/// <summary>
		/// Social rules instantiated from the trait definition.
		/// </summary>
		protected List<SocialRuleInstance> m_socialRuleInstances;

		/// <summary>
		/// Effects instantiated from the trait definition.
		/// </summary>
		protected List<IEffect> m_effects;

		#endregion

		#region Properties

		/// <summary>
		/// The target of this effect.
		/// </summary>
		public IEffectable Target { get; }

		/// <summary>
		/// The unique ID of the trait.
		/// </summary>
		public string TraitID => Definition.TraitID;

		/// <summary>
		/// The name of the trait as displayed in GUIs.
		/// </summary>
		public string DisplayName => Definition.DisplayName;

		/// <summary>
		/// The type of object this trait is applied to.
		/// </summary>
		public TraitType TraitType => Definition.TraitType;

		/// <summary>
		/// A short textual description of the trait.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// IDs of traits that this trait cannot be added with.
		/// </summary>
		public HashSet<string> ConflictingTraits => Definition.ConflictingTraits;

		/// <summary>
		/// Definition data for this instance.
		/// </summary>
		public Trait Definition { get; }

		// From ISocialRuleSource
		public IList<SocialRule> SocialRules => Definition.SocialRules;

		// From ISocialRuleSource
		public IList<SocialRuleInstance> SocialRuleInstances => m_socialRuleInstances;

		#endregion

		#region Constructors

		public TraitInstance(
			IEffectable target,
			Trait trait,
			string description,
			List<IEffect> effects
		)
		{
			Target = target;
			Description = description;
			m_socialRuleInstances = new List<SocialRuleInstance>();
			m_effects = effects;
			Definition = trait;
		}

		#endregion

		#region Public Methods

		public void Apply()
		{
			foreach (var effect in m_effects)
			{
				if (!effect.IsActive)
				{
					effect.Apply();
					effect.IsActive = true;
				}
			}
		}

		public void Remove()
		{
			foreach (var effect in m_effects)
			{
				if (effect.IsActive)
				{
					effect.Remove();
					effect.IsActive = false;
				}
			}

			foreach (var ruleInstance in m_socialRuleInstances)
			{
				ruleInstance.Remove();
			}
		}

		public void TickEffects()
		{
			List<IEffect> effectList = new List<IEffect>(Target.Effects.Effects);

			// Loop backward incase an effect needs to be removed.
			foreach (Effect effect in effectList)
			{
				effect.Tick();

				if (!effect.IsValid)
				{
					effect.Remove();
					effect.IsActive = false;
				}
			}
		}

		public void TickSocialRuleInstances()
		{
			for (int i = m_socialRuleInstances.Count - 1; i >= 0; i--)
			{
				// var instance = m_socialRuleInstances[i];
			}
		}

		// From ISocialRuleSource
		public void AddSocialRuleInstance(SocialRuleInstance instance)
		{
			m_socialRuleInstances.Add(instance);
		}

		// From ISocialRuleSource
		public bool HasSocialRuleInstance(SocialRule rule, string owner, string other)
		{
			foreach (var instance in m_socialRuleInstances)
			{
				if (instance.Rule == rule && instance.Owner == owner && instance.Other == other)
				{
					return true;
				}
			}

			return false;
		}

		// From ISocialRuleSource
		public SocialRuleInstance GetSocialRuleInstance(SocialRule rule, string owner, string other)
		{
			foreach (var instance in m_socialRuleInstances)
			{
				if (instance.Rule == rule && instance.Owner == owner && instance.Other == other)
				{
					return instance;
				}
			}

			throw new KeyNotFoundException(
				$"Could not find social rule instance from {owner} to {other}");
		}

		/// <summary>
		/// Check if the source already contains a given social rule.
		/// </summary>
		/// <param name="rule"></param>
		/// <returns></returns>
		public bool HasSocialRule(SocialRule rule)
		{
			return SocialRules.Contains(rule);
		}

		// From ISocialRuleSource
		public bool RemoveSocialRuleInstance(SocialRuleInstance instance)
		{
			return m_socialRuleInstances.Remove(instance);
		}

		#endregion

		#region Static Methods

		public static TraitInstance CreateInstance(
			SocialEngine engine,
			Trait trait,
			EffectContext context,
			IEffectable target
		)
		{
			List<IEffect> effectList = new List<IEffect>();

			string description = trait.Description;

			// Create the final trait description using the template in the trait definition.
			foreach (var (variableName, value) in context.Bindings)
			{
				description = description.Replace(
					$"[{variableName.Substring(1)}]", value.ToString());
			}

			// Instantiate and apply trait effects.
			foreach (var effectEntry in trait.Effects)
			{
				try
				{
					var effect = engine.EffectLibrary.CreateInstance(context, effectEntry);
					effectList.Add(effect);
				}
				catch (ArgumentException ex)
				{
					throw new ArgumentException(
						$"Error while instantiating effects for '{trait.TraitID}' trait: "
						+ ex.Message
					);
				}
			}

			return new TraitInstance(target, trait, description, effectList);
		}

		#endregion
	}
}
