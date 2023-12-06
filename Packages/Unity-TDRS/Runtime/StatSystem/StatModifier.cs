namespace TDRS.StatSystem
{
    public struct StatModifier
    {
        /// <summary>
        /// The modifier value to apply.
        /// </summary>
        readonly float value;
        /// <summary>
        /// How to mathematically apply the modifier value.
        /// </summary>
        readonly StatModifierType modifierType;
        /// <summary>
        /// The priority of this modifier when calculating final stat values.
        /// </summary>
        readonly int order;
        /// <summary>
        /// The object responsible for applying the modifier.
        /// </summary>
        readonly object? source;

        public StatModifier(
            float value,
            StatModifierType modifierType,
            int? order = null,
            object? source = null
        )
        {
            this.value = value;
            this.modifierType = modifierType;
            this.source = source;
            if (order == null)
            {
                this.order = (int)this.modifierType;
            }
            else
            {
                this.order = -1;
            }
        }
    }
}
