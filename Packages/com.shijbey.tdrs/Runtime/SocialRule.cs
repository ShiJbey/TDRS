namespace TDRS
{
	/// <summary>
	/// Defines effects to apply to a relationship based on a precondition query.
	/// </summary>
	public class SocialRule
	{
		#region Properties

		/// <summary>
		/// A query to run against the social engine's database that needs to pass for the effects
		/// to trigger.
		/// </summary>
		public string[] Preconditions { get; set; }

		/// <summary>
		/// Effects to apply if the precondition passes.
		/// </summary>
		public string[] Effects { get; set; }

		/// <summary>
		/// A template description to be filled when creating instances of this social rule.
		/// </summary>
		public string DescriptionTemplate { get; set; }

		/// <summary>
		/// The object responsible for defining this social rules.
		/// </summary>
		public Trait Source { get; set; }

		#endregion

		#region Constructors

		public SocialRule()
		{
			Preconditions = new string[0];
			Effects = new string[0];
			DescriptionTemplate = "";
			Source = null;
		}

		#endregion
	}
}
