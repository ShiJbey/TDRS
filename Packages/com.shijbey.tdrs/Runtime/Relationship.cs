using System;
using System.Collections.Generic;
using System.IO;

namespace TDRS
{
	/// <summary>
	/// A directed relationship from one agent to another.
	/// </summary>
	public class Relationship
	{
		#region Properties

		/// <summary>
		/// A reference to the manager that owns this relationship.
		/// </summary>
		public SocialEngine Engine { get; }

		/// <summary>
		/// Reference to the owner of the relationship.
		/// </summary>
		public Agent Owner { get; }

		/// <summary>
		/// Reference to the target of the relationship.
		/// </summary>
		public Agent Target { get; }

		/// <summary>
		/// The collection of traits associated with this relationship.
		/// </summary>
		public TraitManager Traits { get; }

		/// <summary>
		/// A collection of stats associated with this relationship.
		/// </summary>
		public StatManager Stats { get; protected set; }

		/// <summary>
		/// Manages all effects applied to this relationship.
		/// </summary>
		public EffectManager Effects { get; }

		#endregion

		#region Events

		/// <summary>
		/// Event invoked when an relationship is ticked.
		/// </summary>
		public event EventHandler OnTick;

		#endregion

		#region Constructors

		public Relationship(SocialEngine engine, Agent owner, Agent target)
		{
			Engine = engine;
			Owner = owner;
			Target = target;
			Traits = new TraitManager();
			Stats = new StatManager();
			Effects = new EffectManager();
			Stats.OnValueChanged += HandleStatChanged;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a trait to the relationship.
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public bool AddTrait(string traitID)
		{
			// Fail if relationship already has the trait
			if (Traits.HasTrait(traitID)) return false;

			Trait trait = Engine.TraitLibrary.Traits[traitID];

			// Error if trait type is not correct
			if (trait.TraitType != TraitType.Relationship)
			{
				throw new ArgumentException(
					$"Trait ({traitID}) and is not a relationship trait."
				);
			}

			// Fail if we have a conflicting trait
			if (Traits.HasConflictingTrait(trait)) return false;

			Traits.AddTrait(trait);

			// Update the traits listed in RePraxis
			Engine.DB.Insert($"{Owner.UID}.relationships.{Target.UID}.traits.{traitID}");

			// Instantiate the effects
			EffectBindingContext ctx = new EffectBindingContext(this, "");
			foreach (var effectEntry in trait.Effects)
			{
				try
				{
					var effect = ctx.Engine.EffectLibrary.CreateInstance(ctx, effectEntry);
					effect.Source = trait;
					Effects.AddEffect(effect);
				}
				catch (ArgumentException ex)
				{
					throw new ArgumentException(
						$"Error encountered while instantiating effects for '{traitID}' trait: "
						+ ex.Message
					);
				}
			}

			// Instantiate social rules and effects
			foreach (var socialRule in trait.SocialRules)
			{
				Owner.SocialRules.AddSocialRule(socialRule);
			}

			return true;
		}

		/// <summary>
		/// Remove a trait from the relationship.
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public bool RemoveTrait(string traitID)
		{
			if (!Traits.HasTrait(traitID)) return false;

			var trait = Traits.GetTrait(traitID);
			Traits.RemoveTrait(trait);
			Engine.DB.Delete($"{Owner.UID}.relationships.{Target.UID}.traits.{traitID}");

			Effects.RemoveAllFromSource(trait);
			Owner.SocialRules.RemoveAllSocialRulesFromSource(trait);

			return true;
		}

		/// <summary>
		/// Advance the simulation by one simulation tick
		/// </summary>
		public virtual void Tick()
		{
			Effects.TickEffects();
			OnTick?.Invoke(this, EventArgs.Empty);
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Callback executed when a stat value changes on the relationship
		/// </summary>
		/// <param name="stats"></param>
		/// <param name="nameAndValue"></param>
		private void HandleStatChanged(object stats, StatManager.OnValueChangedArgs args)
		{
			Engine.DB.Insert(
				$"{Owner.UID}.relationships.{Target.UID}.stats.{args.StatName}!{args.Value}");
		}

		#endregion
	}
}
