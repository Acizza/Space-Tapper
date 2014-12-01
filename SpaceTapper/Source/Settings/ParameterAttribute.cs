using System;

namespace SpaceTapper.Settings
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ParameterAttribute : Attribute
	{
		public string Name;
		public string FullName;
		public string Description;
		public bool ValueNeeded;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpaceTapper.ParameterAttribute"/> class.
		/// </summary>
		/// <param name="name">Parameter name.</param>
		/// <param name="valueNeeded">If set to <c>true</c>, a value is needed for the parameter.</param>
		/// <param name="desc">Parameter description.</param>
		/// <param name="fullName">Full name of the parameter,
		/// set programmatically to allow distinct parameters that share multiple names.</param>
		public ParameterAttribute(string name, bool valueNeeded = true,
			string desc = "None",
			string fullName = "Unknown")
		{
			Name        = name;
			FullName    = fullName;
			Description = desc;
			ValueNeeded = valueNeeded;
		}
	}
}