using System;
using System.Linq;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class BlockSpawner : AEntity
	{
		public List<RectangleShape> Blocks;
		public float BlockSpacing;

		public int MaxBlocks
		{
			get
			{
				return mMaxBlocks;
			}
			set
			{
				if(value < mMaxBlocks)
					Blocks.RemoveRange(Blocks.Count, Blocks.Count - mMaxBlocks - value); // TODO: Throws exception.

				mMaxBlocks = value;
			}
		}

		int mMaxBlocks;
		static Random mRandom;

		static BlockSpawner()
		{
			mRandom = new Random();
		}

		public BlockSpawner(Game instance, int blocks, float spacing = 125) : base(instance)
		{
			Blocks = new List<RectangleShape>(blocks);
			MaxBlocks = blocks;
			BlockSpacing = spacing;

			RespawnBlocks();
		}

		public override void Update(TimeSpan delta)
		{
			var dt = (float)delta.TotalSeconds;

			for(int i = 0; i < Blocks.Count; ++i)
			{
				var block = Blocks[i];

				block.Position = new Vector2f(block.Position.X, block.Position.Y + 150 * dt);

				if(block.Position.Y >= GInstance.Window.Size.Y)
					PositionBlock(i);
			}
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;

			foreach(var block in Blocks)
				target.Draw(block, states);
		}

		public void RespawnBlocks()
		{
			Blocks.Clear();

			var size = GInstance.Window.Size;

			for(int i = 0; i < MaxBlocks; ++i)
			{
				var shape = new RectangleShape(new Vector2f(mRandom.Next(100, (int)(size.X * 0.25f)), 10));
				shape.FillColor = Color.Red;
				shape.Position = new Vector2f(mRandom.Next(0, (int)size.X), 0);

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

		void PositionBlock(int index)
		{
			var b = Blocks[index];
			var s = GInstance.Window.Size;

			b.Position = new Vector2f(b.Position.X, -BlockSpacing * index + mRandom.Next(-15, 15));
		}
	}
}