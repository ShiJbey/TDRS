namespace TDRS
{
	public class OwnerHasTrait : IPrecondition
	{
		protected string _traitID;

		public OwnerHasTrait(string traitID)
		{
			_traitID = traitID;
		}

		public string Description
		{
			get
			{
				return $"relationship target has the {_traitID} trait";
			}
		}

		public bool CheckPrecondition(SocialEntity target)
		{
			return ((TDRSRelationship)target).Owner.Traits.HasTrait(_traitID);
		}
	}
}
