namespace TDRS.Serialization
{
	public class SerializedTraitInstance
	{
		public string traitID { get; set; }
		public int duration { get; set; }
		public string description { get; set; }

		public SerializedTraitInstance()
		{
			traitID = "";
			duration = -1;
			description = "";
		}
	}
}
