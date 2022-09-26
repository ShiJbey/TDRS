using TraitBasedOpinionSystem.Core;

namespace TraitBasedOpinionSystem
{
    public class AutomaticModifier : OpinionModifier
    {

        public AutomaticModifier(int value) :
            base(
                value,
                (Actor subject, Actor target, Opinion opinion) =>
                {
                    return true;
                }
            )
        {
        }

        public override string ToString()
        {
            return $"Opinion of all characters {_value}";
        }
    }
}



