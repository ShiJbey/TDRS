using System.Collections.Generic;
using UnityEngine;

namespace TDRS
{
	[CreateAssetMenu(fileName = "NewNodeSchema", menuName = "Calypso/Node Schema")]
	public class NodeSchemaScriptableObj : ScriptableObject
	{
		public string nodeType;
		public List<StatSchema> stats;

		public NodeSchema GetSchema()
		{
			return new NodeSchema(nodeType, stats);
		}
	}
}
