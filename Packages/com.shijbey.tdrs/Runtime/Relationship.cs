using System;
using System.Collections.Generic;
using RePraxis;

namespace TDRS
{
	/// <summary>
	/// A directed relationship from one agent to another.
	/// </summary>
	public class Relationship : ISocialEntity
	{
		#region Fields

		/// <summary>
		/// Social rules currently applied to this relationship.
		/// </summary>
		protected List<ActiveSocialRuleEntry> m_activeSocialRules;

		#endregion

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
		/// The trait associated with this relationship that specifies the
		/// general nature of this relationship. (Allows for one mutually-exclusive
		/// relationship stats)
		/// </summary>
		public Trait RelationshipType { get; protected set; }

		/// <summary>
		/// Social rules currently applied to this relationship.
		/// </summary>
		public IEnumerable<ActiveSocialRuleEntry> ActiveSocialRules => m_activeSocialRules;

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
			Traits = new TraitManager(this);
			Stats = new StatManager();
			m_activeSocialRules = new List<ActiveSocialRuleEntry>();
			Stats.OnValueChanged += HandleStatChanged;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a trait to the relationship.
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public bool AddTrait(string traitID, int duration = -1, string descriptionOverride = "")
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

			string description = trait.Description
				.Replace($"[owner]", Owner.UID)
				.Replace($"[target]", Target.UID);

			if (descriptionOverride != "")
			{
				description = descriptionOverride;
			}

			// Add trait and apply effects.
			Traits.AddTrait(trait, description, duration);

			// Update the traits listed in RePraxis database.
			Engine.DB.Insert($"{Owner.UID}.relationships.{Target.UID}.traits.{traitID}");

			// Reevaluate social rules for this relationship incase any depend on the new trait.
			ReevaluateSocialRules();

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

			Traits.RemoveTrait(traitID);

			Trait trait = Engine.TraitLibrary.Traits[traitID];

			if (RelationshipType == trait)
			{
				RelationshipType = null;
				Engine.DB.Delete($"{Owner.UID}.relationships.{Target.UID}.type!{traitID}");
			}

			Engine.DB.Delete($"{Owner.UID}.relationships.{Target.UID}.traits.{traitID}");

			// Reevaluate social rules for this relationship incase any depend on the removed trait.
			ReevaluateSocialRules();

			return true;
		}

		/// <summary>
		/// Set the relationship type to a new trait
		/// </summary>
		/// <param name="traitID"></param>
		public void SetRelationshipType(string traitID)
		{
			if (RelationshipType != null)
			{
				RemoveTrait(RelationshipType.TraitID);
			}

			Trait trait = Engine.TraitLibrary.Traits[traitID];
			RelationshipType = trait;
			Engine.DB.Insert($"{Owner.UID}.relationships.{Target.UID}.type!{traitID}");

			AddTrait(traitID);
		}

		/// <summary>
		/// Advance the simulation by one simulation tick
		/// </summary>
		public void Tick()
		{
			TickTraits();

			OnTick?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Update the traits associated with this relationship.
		/// </summary>
		public void TickTraits()
		{
			List<TraitInstance> traitInstances = new List<TraitInstance>(Traits.Traits);
			foreach (var instance in traitInstances)
			{
				instance.Tick();

				if (instance.HasDuration && instance.Duration <= 0)
				{
					RemoveTrait(instance.TraitID);
				}
			}
		}

		/// <summary>
		/// Recalculates the stats of this relationship instance by reevaluating social rules.
		/// </summary>
		public void ReevaluateSocialRules()
		{
			foreach (var entry in m_activeSocialRules)
			{
				entry.Rule.RemoveModifiers(this);
			}

			m_activeSocialRules.Clear();

			foreach (var rule in Engine.SocialRules)
			{
				var results = new DBQuery(rule.Preconditions).Run(
					Engine.DB,
					new Dictionary<string, object>()
					{
						{"?owner", Owner.UID},
						{"?target", Target.UID}
					}
				);

				if (!results.Success) continue;

				rule.ApplyModifiers(this);

				m_activeSocialRules.Add(
					new ActiveSocialRuleEntry(
						rule,
						rule.Description
							.Replace($"[owner]", Owner.UID)
							.Replace($"[target]", Target.UID)
					)
				);
			}
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

	public class ActiveSocialRuleEntry
	{
		public SocialRule Rule { get; }
		public string Description { get; }

		public ActiveSocialRuleEntry(SocialRule rule, string description)
		{
			Rule = rule;
			Description = description;
		}
	}
}
