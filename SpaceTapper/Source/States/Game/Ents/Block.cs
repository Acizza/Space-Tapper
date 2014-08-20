using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class Block : ARectEntity
	{
		public bool Scored;

		public Block(Game instance) : base(instance)
		{
		}

		public Block(Game instance, Vector2f size) : base(instance, size)
		{
		}
	}
}