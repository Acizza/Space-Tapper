using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class Block : ARectEntity
	{
		public bool Scored;

		public Block(AState state) : base(state)
		{
		}

		public Block(AState state, Vector2f size) : base(state, size)
		{
		}
	}
}