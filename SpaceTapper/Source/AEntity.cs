using System;
using SFML.Graphics;

namespace SpaceTapper
{
	public abstract class AEntity : SceneNode
	{
		public Game GInstance;

		public AEntity(Game instance)
		{
			GInstance = instance;
		}
	}
}