using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class Block : RectangleShape
	{
		public bool Scored;

		public Block() : base()
		{
		}

		public Block(Vector2f pos) : base(pos)
		{
		}
	}
}