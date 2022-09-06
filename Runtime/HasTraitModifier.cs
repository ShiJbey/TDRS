using TraitBasedOpinionSystem.Core;

namespace TraitBasedOpinionSystem
{
    public class HasTraitModifier : OpinionModifier
    {
        public HasTraitModifier(string traitName, int value):
            base(
                value, 
                (Actor subject, Actor target, Opinion opinion) =>
                {
                    return target.HasTrait(traitName);
                }
            )
        {}
    }
}


