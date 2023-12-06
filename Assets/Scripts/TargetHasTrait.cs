using UnityEngine;

namespace TDRS.Sample
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

        public bool CheckPrecondition(GameObject target)
        {
            return target.GetComponent<SocialRelationship>().Target.HasTrait(_traitID);
        }
    }
}


