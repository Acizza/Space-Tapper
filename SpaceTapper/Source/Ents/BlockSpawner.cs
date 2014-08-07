using System;
using System.Linq;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class BlockSpawner : Transformable, Drawable
	{
		public List<RectangleShape> Blocks { get; private set; }
		public float BlockSpacing;

		public int MaxBlocks
		{
			get
			{
				return maxBlocks;
			}
			set
			{
				if(value < maxBlocks)
					Blocks.RemoveRange(Blocks.Count, Blocks.Count - maxBlocks - value); // TODO: Throws exception.

				maxBlocks = value;
			}
		}

		private int maxBlocks;
		private static Random random;

		static BlockSpawner()
		{
			random = new Random();
		}

		public BlockSpawner(int blocks, float spacing = 125)
		{
			Blocks = new List<RectangleShape>(blocks);
			maxBlocks = blocks;
			BlockSpacing = spacing;

			RespawnBlocks();
		}

		public void Update(float dt)
		{
			for(int i = 0; i < Blocks.Count; ++i)
			{
				var block = Blocks[i];

				block.Position = new Vector2f(block.Position.X, block.Position.Y + 150 * dt);

				if(block.Position.Y >= Game.Instance.Window.Size.Y)
					PositionBlock(i);
			}
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;

			foreach(var block in Blocks)
				target.Draw(block, states);
		}

		public void RespawnBlocks()
		{
			Blocks.Clear();

			var size = Game.Instance.Window.Size;

			for(int i = 0; i < MaxBlocks; ++i)
			{
				var shape = new RectangleShape(new Vector2f(random.Next(100, (int)(size.X * 0.25f)), 10));
				shape.FillColor = Color.Red;
				shape.Position = new Vector2f(random.Next(0, (int)size.X), 0);

				Blocks.Add(shape);
				PositionBlock(i);
			}
		}

		public bool CheckCollision(FloatRect rect)
		{
			foreach(var block in Blocks)
			{
				if(block.GetGlobalBounds().Intersects(rect))
					return true;
			}

			return false;
		}

		private void PositionBlock(int index)
		{
			var b = Blocks[index];
			var s = Game.Instance.Window.Size;

			b.Position = new Vector2f(b.Position.X, -BlockSpacing * index + random.Next(-15, 15));
		}
	}
}