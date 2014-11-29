using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper.Entities
{
	public sealed class Block : RectangleShape, IResetable
	{
		public bool Passed;

		public Block()
		{
		}

		public Block(Vector2f size) : base(size)
		{
		}

		public void Reset()
		{
			Passed = false;
		}
	}
}