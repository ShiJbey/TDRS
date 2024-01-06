#nullable enable

using System.Collections.Generic;
using YamlDotNet.Helpers;
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
					throw new KeyNotFoundException($"Missing attribute '{childID}'.");
				}

				return childNode;
			}

			throw new System.Exception("Cannot call GetChild() on non-mapping node.");
		}

		/// <summary>
		/// Gets a child node from a mapping using its string name
		/// </summary>
		/// <param name="mapping"></param>
		/// <param name="childID"></param>
		/// <returns></returns>
		public static YamlNode TryGetChild(this YamlNode node, string childID)
		{
			if (node.NodeType == YamlNodeType.Mapping)
			{
				var mapping = (YamlMappingNode)node;

				YamlNode childNode;
				mapping.Children.TryGetValue(new YamlScalarNode(childID), out childNode);

				return childNode;
			}

			throw new System.Exception("Cannot call GetChild() on non-mapping node.");
		}

		/// <summary>
		/// Try to get a child node from a mapping using its string name
		/// </summary>
		/// <param name="node"></param>
		/// <param name="childID"></param>
		/// <param name="childNode"></param>
		/// <returns></returns>
		public static bool TryGetChild(this YamlNode node, string childID, out YamlNode childNode)
		{
			if (node.NodeType == YamlNodeType.Mapping)
			{
				var mapping = (YamlMappingNode)node;

				var success = mapping.Children.TryGetValue(
					new YamlScalarNode(childID), out childNode);

				return success;
			}

			throw new System.Exception("Cannot call GetChild() on non-mapping node.");
		}

		/// <summary>
		/// Gets a child node from a mapping using its string name
		/// </summary>
		/// <param name="mapping"></param>
		/// <param name="childID"></param>
		/// <returns></returns>
		public static IOrderedDictionary<YamlNode, YamlNode> GetChildren(this YamlNode node)
		{
			if (node.NodeType == YamlNodeType.Mapping)
			{
				var mapping = (YamlMappingNode)node;

				return mapping.Children;
			}

			throw new System.Exception("Cannot call GetChildren() on non-mapping node.");
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
