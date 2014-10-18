using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper.Ents
{
	public class Block : RectangleShape
	{
		public bool Passed;

		public Block()
		{
		}

		public Block(Vector2f size) : base(size)
		{
		}
	}
}