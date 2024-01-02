using System.Linq;

namespace TDRS
{
	public abstract class SocialEntity
	{
		#region Properties

		/// <summary>
		/// Get the unique ID of the entity
		/// </summary>
		public string UID { get; }

		/// <summary>
		/// A reference to the manager that owns this entity
		/// </summary>
		public SocialEngine Engine { get; }

		/// <summary>
		/// The collection of traits associated with this entity
		/// </summary>
		public TraitCollection Traits { get; protected set; }

		/// <summary>
		/// A collection of stats associated with this entity
		/// </summary>
		public StatCollection Stats { get; protected set; }

		/// <summary>
		/// All social rules affecting this entity
		/// </summary>
		public SocialRules SocialRules { get; protected set; }

		#endregion

		#region Constructors

		public SocialEntity(
			SocialEngine engine,
			string entityID
		)
		{
			Engine = engine;
			UID = entityID;
			Traits = new TraitCollection();
			Stats = new StatCollection();
			SocialRules = new SocialRules();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add a trait to an entity.
		/// </summary>
		/// <param name="traitID"></param>
		public virtual void AddTrait(string traitID)
		{
			var trait = Engine.TraitLibrary.GetTrait(traitID);
			Traits.AddTrait(trait);
			trait.OnAdd(this);
		}

		/// <summary>
		/// Remove a trait from the entity.
		/// </summary>
		/// <param name="traitID"></param>
		public virtual void RemoveTrait(string traitID)
		{
			var trait = Engine.TraitLibrary.GetTrait(traitID);
			Traits.RemoveTrait(trait);
			trait.OnRemove(this);
		}

		/// <summary>
		/// Remove a social rule from a node
		/// </summary>
		/// <param name="socialRule"></param>
		public virtual void AddSocialRule(SocialRule socialRule)
		{
			SocialRules.AddSocialRule(socialRule);
		}

		/// <summary>
		/// Remove a social rule from a node
		/// </summary>
		/// <param name="socialRule"></param>
		public virtual void RemoveSocialRule(SocialRule socialRule)
		{
			SocialRules.RemoveSocialRule(socialRule);
		}

		/// <summary>
		/// Remove all social rules on a node from a given source
		/// </summary>
		/// <param name="socialRule"></param>
		public void RemoveAllSocialRulesFromSource(object source)
		{
			var socialRules = SocialRules.Rules.ToList();
			for (int i = socialRules.Count(); i >= 0; i--)
			{
				var rule = socialRules[i];
				if (rule.Source == source)
				{
					RemoveSocialRule(rule);
				}
			}
		}

		#endregion
	}
}
