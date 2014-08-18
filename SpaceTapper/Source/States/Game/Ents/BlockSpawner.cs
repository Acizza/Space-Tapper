using System;
using System.Linq;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class BlockSpawner : AEntity
	{
		public List<Block> Blocks { get; private set; }

		DifficultySettings mDifficulty;
		int mMaxBlocks;

		static Random mRand;

		public int MaxBlocks
		{
			get
			{
				return mMaxBlocks;
			}
			set
			{
				if(value < mMaxBlocks)
					Blocks.RemoveRange(Blocks.Count - value, value);
				else if(value > mMaxBlocks)
					CreateBlocks(value - mMaxBlocks);

				mMaxBlocks = value;
			}
		}

		public DifficultySettings Difficulty
		{
			get
			{
				return mDifficulty;
			}
			set
			{
				mDifficulty = value;
				MaxBlocks = value.BlockCount;
			}
		}

		static BlockSpawner()
		{
			mRand = new Random();
		}

		public BlockSpawner(Game instance) : base(instance)
		{
			Blocks = new List<Block>();
		}

		public override void Update(TimeSpan delta)
		{
			var dt = (float)delta.TotalSeconds;
			int index = 0;

			foreach(var block in Blocks)
			{
				++index;
				block.Position += new Vector2f(0, Difficulty.BlockSpeed * dt);

				if(block.Position.Y >= GInstance.Size.Y)
					ResetBlock(block, index);
			}
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;

			foreach(var block in Blocks)
				target.Draw(block, states);
		}

		public void CreateBlocks(int count)
		{
			var size = GInstance.Size;

			for(int i = 0; i < count; ++i)
			{
				var block = new Block(new Vector2f(mRand.Next(100, (int)(size.X * 0.25f)), 10));
				block.FillColor = Color.Red;

				// Prevent intersections.
				FloatRect lastPos;

				do
				{
					lastPos = block.GetGlobalBounds();
					ResetBlock(block, i);
				}
				while(block.GetGlobalBounds().Intersects(lastPos));

				Blocks.Add(block);
			}
		}

		public void ResetBlock(Block block, int heightMult)
		{
			block.Position = new Vector2f(mRand.Next(0, (int)GInstance.Size.X),
				-Difficulty.BlockSpacing * heightMult + mRand.Next(-15, 15));

			block.Scored = false;
		}

		public void Reset()
		{
			Blocks.Clear();
			mMaxBlocks = 0;
		}
	}
}