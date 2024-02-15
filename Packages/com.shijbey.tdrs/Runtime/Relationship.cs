using System;
using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// A directed relationship from one agent to another.
	/// </summary>
	public class Relationship : IEffectable
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
			Trait trait = Engine.TraitLibrary.Traits[traitID];

			// Fail if relationship already has the trait
			if (Traits.HasTrait(trait)) return false;

			// Fail if we have a conflicting trait
			if (Traits.HasConflictingTrait(trait)) return false;

			// Error if trait type is not correct
			if (trait.TraitType != TraitType.Relationship)
			{
				throw new ArgumentException(
					$"Trait ({traitID}) and is not a relationship trait."
				);
			}

			// Instantiate the effects
			EffectContext ctx = new EffectContext(
				Engine,
				"",
				new Dictionary<string, object>()
				{
					{ "?owner", Owner.UID },
					{ "?target", Target.UID },
				},
				trait
			);

			TraitInstance traitInstance = TraitInstance.CreateInstance(Engine, trait, ctx, this);

			Traits.AddTrait(traitInstance);

			// Update the traits listed in RePraxis
			Engine.DB.Insert($"{Owner.UID}.relationships.{Target.UID}.traits.{traitID}");

			traitInstance.Apply();

			Owner.SocialRules.AddSource(traitInstance);

			Owner.ReevaluateRelationships();
			Target.ReevaluateRelationships();

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

			var traitInstance = Traits.GetTrait(traitID);

			Traits.RemoveTrait(traitID);

			Engine.DB.Delete($"{Owner.UID}.relationships.{Target.UID}.traits.{traitID}");

			traitInstance.Remove();

			Owner.SocialRules.RemoveSource(traitInstance);

			Owner.ReevaluateRelationships();
			Target.ReevaluateRelationships();

			return true;
		}

		/// <summary>
		/// Advance the simulation by one simulation tick
		/// </summary>
		public virtual void Tick()
		{
			foreach (var traitInstance in Traits.Traits)
			{
				Effects.TickEffects();
				traitInstance.TickSocialRuleInstances();
			}

			OnTick?.Invoke(this, EventArgs.Empty);
		}

		public override string ToString()
		{
			return $"Relationship({Owner}, {Target})";
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
