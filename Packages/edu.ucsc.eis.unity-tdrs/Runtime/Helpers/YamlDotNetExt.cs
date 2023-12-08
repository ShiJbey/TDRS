#nullable enable

using YamlDotNet.RepresentationModel;

namespace TDRS.Helpers
{
	public static class YamlDotNetExt
	{
		/// <summary>
		/// Gets a child node from a mapping using its string name
		/// </summary>
		/// <param name="mapping"></param>
		/// <param name="childID"></param>
		/// <returns></returns>
		public static YamlNode GetChild(this YamlNode node, string childID)
		{
			if (node.NodeType == YamlNodeType.Mapping)
			{
				var mapping = (YamlMappingNode)node;

				YamlNode childNode;
				mapping.Children.TryGetValue(new YamlScalarNode(childID), out childNode);
				if (childNode == null)
				{
					throw new System.Exception($"Missing attribute '{childID}'.");
				}

				return childNode;
			}

			throw new System.Exception("Cannot call GetChild() on non-mapping node.");
		}

		/// <summary>
		/// Returns the value of the node and throws an exception if the value is null
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static string GetValue(this YamlNode node)
		{
			if (node.NodeType == YamlNodeType.Scalar)
			{
				var scalar = (YamlScalarNode)node;

				string? valueString = scalar.Value;

				if (valueString == null)
				{
					throw new System.Exception("Scalar node value is null.");
				}

				return valueString;
			}

			throw new System.Exception("Cannot call GetChild() on non-mapping node.");
		}

		/// <summary>
		/// Returns the value of the node and throws an exception if the value is null
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static string GetValue(this YamlNode node, string fallback)
		{
			if (node.NodeType == YamlNodeType.Scalar)
			{
				var scalar = (YamlScalarNode)node;

				string? valueString = scalar.Value;

				if (valueString == null)
				{
					return fallback;
				}

				return valueString;
			}

			throw new System.Exception("Cannot call GetChild() on non-mapping node.");
		}
	}
}
