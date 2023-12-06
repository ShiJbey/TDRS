namespace TDRS
{
    public interface IPreconditionFactory
    {
        /// <summary>
        /// Construct a new instance of a Precondition.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public IPrecondition Instantiate(params object?[] args);
    }
}
