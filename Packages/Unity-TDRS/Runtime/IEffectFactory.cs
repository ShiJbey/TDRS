namespace TDRS
{
    public interface IEffectFactory
    {
        /// <summary>
        /// Construct a new instance of an Effect.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public IEffect Instantiate(params object?[] args);
    }
}

