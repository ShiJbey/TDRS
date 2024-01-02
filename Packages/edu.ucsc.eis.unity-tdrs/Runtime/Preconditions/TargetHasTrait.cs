namespace TDRS
{
	public class TargetHasTrait : IPrecondition
	{
		protected string _traitID;

		public TargetHasTrait(string traitID)
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
			return ((SocialRelationship)target).Target.Traits.HasTrait(_traitID);
		}
	}
}
