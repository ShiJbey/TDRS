using TraitBasedOpinionSystem.Core;

namespace TraitBasedOpinionSystem
{
    public class HasTraitModifier : OpinionModifier
    {
        protected string _traitName;

        public HasTraitModifier(string traitName, int value):
            base(
                value, 
                (Actor subject, Actor target, Opinion opinion) =>
                {
                    return target.HasTrait(traitName);
                }
            )
        {
            _traitName = traitName;
        }

        public override string ToString()
        {
            return $"Opinion of {_traitName} characters {_value}";
        }
    }
}


