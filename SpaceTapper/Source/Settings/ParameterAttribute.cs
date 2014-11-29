using System;

namespace SpaceTapper
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ParameterAttribute : Attribute
	{
		public string Name;
		public string Description;
		public bool ValueNeeded;

		public ParameterAttribute(string name, bool valueNeeded = true, string desc = "None")
		{
			Name        = name;
			Description = desc;
			ValueNeeded = valueNeeded;
		}
	}
}