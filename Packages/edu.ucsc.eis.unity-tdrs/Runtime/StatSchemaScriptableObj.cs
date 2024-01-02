using UnityEngine;

namespace TDRS
{
	/// <summary>
	/// Holds a collection of schema for configuring stats on an entity
	/// or relationship.
	/// </summary>
	[CreateAssetMenu(menuName = "TDRS/Stat Schema", fileName = "New Stat Schema")]
	public class StatSchemaScriptableObj : ScriptableObject
	{
		public StatSchema[] stats;
	}
}
