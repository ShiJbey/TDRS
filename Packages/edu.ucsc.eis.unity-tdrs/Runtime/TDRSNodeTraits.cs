namespace TDRS
{
	/// <summary>
	/// A traits class with behavior specialized for working with TDRSNodes
	/// </summary>
	public class TDRSNodeTraits : Traits
	{
		protected TDRSNode _node;

		public TDRSNodeTraits(TDRSNode node) : base()
		{
			_node = node;
		}

		public override bool AddTrait(Trait trait)
		{
			bool success = base.AddTrait(trait);

			if (success && _node.GameObject != null)
			{
				var tdrsEntity = _node.GameObject.GetComponent<TDRSEntity>();
				if (tdrsEntity != null)
				{
					tdrsEntity.OnTraitAdded.Invoke(trait);
				}
			}

			return success;
		}

		public override bool RemoveTrait(Trait trait)
		{
			bool success = base.RemoveTrait(trait);

			if (success && _node.GameObject != null)
			{
				var tdrsEntity = _node.GameObject.GetComponent<TDRSEntity>();
				if (tdrsEntity != null)
				{
					tdrsEntity.OnTraitRemoved.Invoke(trait);
				}
			}

			return success;
		}
	}
}
