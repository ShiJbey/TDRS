namespace TraitBasedOpinionSystem.Core
{
    public delegate bool OpinionModifierPrecondition(Actor subject, Actor target, Opinion opinion);

    /// <summary>
    /// Opinion modifiers objects that affect the final value of
    /// one NPC's opinion of another.
    /// 
    /// Each modifier has a precondition that needs to be met. If
    /// the preconditions are met, then the Opinion modifier is
    /// added to an Opinion object and it's value is used for
    /// calculations.
    /// </summary>
    public class OpinionModifier
    {
        protected readonly float _value;

        public float Value { get { return _value; } }

        /// <summary>
        /// Delegate function that returns true if the
        /// given character meets its preconditions
        /// </summary>
        protected readonly OpinionModifierPrecondition _precondition;


        public OpinionModifierPrecondition Precondition 
        {
            get { return _precondition; }
        }


        public OpinionModifier(float value)
        {
            _value = value;
            _precondition = null;
        }

        public OpinionModifier(float value, OpinionModifierPrecondition precondition)
        {
            _value = value;
            _precondition = precondition;
        }
    }
}
