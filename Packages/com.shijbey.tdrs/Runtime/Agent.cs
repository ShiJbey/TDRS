using System;
using System.Collections.Generic;

namespace TDRS
{
	/// <summary>
	/// An entity within a social graph that is connected to other agents via relationships.
	/// </summary>
	public class Agent : ISocialEntity
	{
		#region Properties

		/// <summary>
		/// Get the unique ID of the agent.
		/// </summary>
		public string UID { get; }

		/// <summary>
		/// The schema name name of this agent.
		/// </summary>
		public string AgentType { get; }

		/// <summary>
		/// A reference to the manager that owns this agent.
		/// </summary>
		public SocialEngine Engine { get; }

		/// <summary>
		/// The collection of traits associated with this agent.
		/// </summary>
		public TraitManager Traits { get; }

		/// <summary>
		/// A collection of stats associated with this agent.
		/// </summary>
		public StatManager Stats { get; }

		/// <summary>
		/// Relationships directed toward this agent.
		/// </summary>
		public Dictionary<Agent, Relationship> IncomingRelationships { get; }

		/// <summary>
		/// Relationships from this agent directed toward other agents.
		/// </summary>
		public Dictionary<Agent, Relationship> OutgoingRelationships { get; }

		#endregion

		#region Events

		/// <summary>
		/// Event invoked when an agent is ticked.
		/// </summary>
		public event EventHandler OnTick;

		#endregion

		#region Constructors

		public Agent(SocialEngine engine, string uid, string agentType)
		{
			UID = uid;
			AgentType = agentType;
			Engine = engine;
			Traits = new TraitManager(this);
			Stats = new StatManager();
			OutgoingRelationships = new Dictionary<Agent, Relationship>();
			IncomingRelationships = new Dictionary<Agent, Relationship>();

			Stats.OnValueChanged += HandleStatChanged;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a trait to the agent.
		/// </summary>
		/// <param name="traitID"></param>
		/// <param name="duration"></param>
		/// <returns></returns>
		public bool AddTrait(string traitID, int duration = -1, string descriptionOverride = "")
		{
			Trait trait = Engine.TraitLibrary.Traits[traitID];

			if (Traits.HasTrait(trait)) return false;

			if (Traits.HasConflictingTrait(trait)) return false;

			if (trait.TraitType != TraitType.Agent)
			{
				throw new ArgumentException(
					$"Trait ({traitID}) must be of type 'Agent'."
				);
			}

			string description = trait.Description.Replace($"[owner]", UID);

			if (descriptionOverride != "")
			{
				description = descriptionOverride;
			}

			Traits.AddTrait(trait, description, duration);

			Engine.DB.Insert($"{UID}.traits.{traitID}");

			ReevaluateRelationships();

			return true;
		}

		/// <summary>
		/// Remove a trait from the agent.
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public bool RemoveTrait(string traitID)
		{
			if (!Traits.HasTrait(traitID)) return false;

			Traits.RemoveTrait(traitID);

			Engine.DB.Delete($"{UID}.traits.{traitID}");

			ReevaluateRelationships();

			return true;
		}

		/// <summary>
		/// Advance the simulation by one simulation tick
		/// </summary>
		public void Tick()
		{
			TickTraits();
			ReevaluateRelationships();

			OnTick?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Update the traits associated with this agent.
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
		/// Reevaluate relationships against the social rules.
		/// </summary>
		public void ReevaluateRelationships()
		{
			foreach (var (_, relationship) in OutgoingRelationships)
			{
				relationship.ReevaluateSocialRules();
			}

			foreach (var (_, relationship) in IncomingRelationships)
			{
				relationship.ReevaluateSocialRules();
			}
		}

		public void ReevaluateRelationship(Agent target)
		{
			OutgoingRelationships[target].ReevaluateSocialRules();
		}

		public override string ToString()
		{
			return $"Agent({UID})";
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Callback executed when an agent's stat changes its value.
		/// </summary>
		/// <param name="stats"></param>
		/// <param name="nameAndValue"></param>
		private void HandleStatChanged(object stats, StatManager.OnValueChangedArgs args)
		{
			Engine.DB.Insert($"{UID}.stats.{args.StatName}!{args.Value}");
		}

		#endregion
	}
}
