using UnityEngine;

namespace TDRS.Sample
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

        public bool CheckPrecondition(GameObject target)
        {
            return target.GetComponent<SocialRelationship>().Owner.HasTrait(_traitID);
        }
    }
}