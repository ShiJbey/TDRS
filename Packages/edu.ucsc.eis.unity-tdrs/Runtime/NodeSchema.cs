using System.Collections.Generic;
using System.Linq;

namespace TDRS
{
	public class NodeSchema
	{
		public readonly string nodeType;
		public readonly StatSchema[] stats;

		public NodeSchema(string nodeType, IEnumerable<StatSchema> stats)
		{
			this.nodeType = nodeType;
			this.stats = stats.ToArray();
		}
	}

	[System.Serializable]
	public class StatSchema
	{
		public string statName;
		public float baseValue;
		public float maxValue;
		public float minValue;
		public bool isDiscrete;

		public StatSchema(
			string statName,
			float baseValue,
			float maxValue,
			float minValue,
			bool isDiscrete)
		{
			this.statName = statName;
			this.baseValue = baseValue;
			this.maxValue = maxValue;
			this.minValue = minValue;
			this.isDiscrete = isDiscrete;
		}
	}

	public class RelationshipSchema
	{
		public string ownerType;
		public string targetType;
		public StatSchema[] stats;

		public RelationshipSchema(string ownerType, string targetType, IEnumerable<StatSchema> stats)
		{
			this.ownerType = ownerType;
			this.targetType = targetType;
			this.stats = stats.ToArray();
		}
	}
}
