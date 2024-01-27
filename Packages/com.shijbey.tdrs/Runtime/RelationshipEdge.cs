using System;
using System.Collections.Generic;
using TDRS.StatSystem;

namespace TDRS
{
	/// <summary>
	/// A directed edge in a <c>SocialGraph</c> connecting one <c>AgentNode</c> to another.
	/// </summary>
	public class RelationshipEdge
	{
		#region Properties

		/// <summary>
		/// A reference to the manager that owns this entity
		/// </summary>
		public SocialEngine Engine { get; }

		/// <summary>
		/// Reference to the owner of the relationship
		/// </summary>
		public AgentNode Owner { get; }

		/// <summary>
		/// Reference to the target of the relationship
		/// </summary>
		public AgentNode Target { get; }

		/// <summary>
		/// The collection of traits associated with this entity
		/// </summary>
		public TraitManager Traits { get; }

		/// <summary>
		/// A collection of stats associated with this entity
		/// </summary>
		public StatManager Stats { get; protected set; }

		#endregion

		#region Events

		/// <summary>
		/// Event invoked when an entity is ticked;
		/// </summary>
		public event EventHandler OnTick;

		#endregion

		#region Constructors

		public RelationshipEdge(SocialEngine engine, AgentNode owner, AgentNode target)
		{
			Engine = engine;
			Owner = owner;
			Target = target;
			Traits = new TraitManager();
			Stats = new StatManager();
			Stats.OnValueChanged += HandleStatChanged;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a trait to the relationship.
		/// </summary>
		/// <param name="traitID"></param>
		/// <param name="duration"></param>
		public void AddTrait(string traitID, int duration = -1)
		{
			if (Traits.HasTrait(traitID)) return;

			Trait trait = Engine.TraitLibrary.CreateInstance(traitID, this);
			Traits.AddTrait(trait, duration);
			Engine.DB.Insert($"{Owner.UID}.relationships.{Target.UID}.traits.{traitID}");

			// Apply the trait's effects on the owner
			foreach (var effect in trait.Effects)
			{
				effect.Apply();
			}

			// Add the social rules for this trait
			foreach (var socialRuleDef in trait.SocialRuleDefinitions)
			{
				Owner.SocialRules.AddSocialRuleDefinition(socialRuleDef);
			}
		}

		/// <summary>
		/// Remove a trait from the relationship.
		/// </summary>
		/// <param name="traitID"></param>
		public void RemoveTrait(string traitID)
		{
			if (!Traits.HasTrait(traitID)) return;

			var trait = Traits.GetTrait(traitID);
			Traits.RemoveTrait(trait);
			Engine.DB.Delete($"{Owner.UID}.relationships.{Target.UID}.traits.{traitID}");

			// Undo the effects of the trait on the owner
			foreach (var effect in trait.Effects)
			{
				effect.Remove();
			}

			// Remove social rules from the relationship owner
			foreach (var socialRuleDef in trait.SocialRuleDefinitions)
			{
				Owner.SocialRules.RemoveSocialRuleDefinition(socialRuleDef);
			}
		}

		/// <summary>
		/// Advance the simulation by one simulation tick
		/// </summary>
		public virtual void Tick()
		{
			TickTraits();
			TickStats();

			OnTick?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Update the stats and modifiers by one simulation tick
		/// </summary>
		public void TickStats()
		{
			List<StatModifier> modifiers = new List<StatModifier>(Stats.Modifiers);

			// Loop backward since we may remove items from the list
			foreach (var modifier in modifiers)
			{
				if (modifier.Duration > 0)
				{
					modifier.DecrementDuration();
				}

				if (modifier.Duration == 0)
				{
					Stats.RemoveModifier(modifier);
				}
			}
		}

		/// <summary>
		/// Tick update the traits of characters
		/// </summary>
		public void TickTraits()
		{
			List<Trait> traits = Traits.Traits;

			// Loop backward since we may remove items from the list
			for (int i = traits.Count - 1; i >= 0; i--)
			{
				var trait = traits[i];

				if (trait.Duration > 0)
				{
					trait.DecrementDuration();
				}

				if (trait.Duration == 0)
				{
					RemoveTrait(trait.TraitID);
				}
			}
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Callback executed when a stat value changes on the relationship
		/// </summary>
		/// <param name="stats"></param>
		/// <param name="nameAndValue"></param>
		private void HandleStatChanged(object stats, (string, float) nameAndValue)
		{
			string statName = nameAndValue.Item1;
			float value = nameAndValue.Item2;
			Engine.DB.Insert($"{Owner.UID}.relationships.{Target.UID}.stats.{statName}!{value}");
		}

		#endregion
	}
}
