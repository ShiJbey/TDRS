namespace TDRS
{
	/// <summary>
	/// A precondition is an object that defines requirements for effects to happen.
	/// </summary>
	public interface IPrecondition
	{
		/// <summary>
		/// Get a textual description of the precondition.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Check if the GameObject passes the preconditions.
		/// </summary>
		public bool CheckPrecondition(SocialEntity target);
	}
}
