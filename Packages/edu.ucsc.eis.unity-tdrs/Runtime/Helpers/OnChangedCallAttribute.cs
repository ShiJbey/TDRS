using UnityEngine;

public class OnChangedCallAttribute : PropertyAttribute
{
	public string methodName;
	public OnChangedCallAttribute(string methodNameNoArguments)
	{
		methodName = methodNameNoArguments;
	}
}
