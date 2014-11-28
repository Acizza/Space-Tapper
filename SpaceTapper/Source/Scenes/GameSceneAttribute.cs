using System;

namespace SpaceTapper.Scenes
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class GameSceneAttribute : Attribute
	{
		public string Name { get; private set; }

		public GameSceneAttribute(string name)
		{
			Name = name;
		}
	}
}