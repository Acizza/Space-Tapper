using System;

namespace SpaceTapper.States
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class StateAttribute : Attribute
	{
		public StateAttribute()
		{
		}
	}
}