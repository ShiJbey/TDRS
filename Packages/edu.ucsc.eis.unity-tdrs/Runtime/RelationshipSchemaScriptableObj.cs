using System.Collections.Generic;
using UnityEngine;

namespace TDRS
{
	[CreateAssetMenu(fileName = "NewRelationshipSchema", menuName = "Calypso/Relationship Schema")]
	public class RelationshipSchemaScriptableObj : ScriptableObject
	{
		public string ownerType;
		public string targetType;
		public List<StatSchema> stats;

		public RelationshipSchema GetSchema()
		{
			return new RelationshipSchema(ownerType, targetType, stats);
		}
	}
}
