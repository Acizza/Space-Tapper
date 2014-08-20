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
		public int UpgradeChance = 15;

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

		public override void UpdateSelf(TimeSpan delta)
		{
			var dt = (float)delta.TotalSeconds;

			for(int i = 0; i < Blocks.Count; ++i)
			{
				var block = Blocks[i];

				block.Position += new Vector2f(0, Difficulty.BlockSpeed * dt);

				if(block.Position.Y >= GInstance.Size.Y)
					ResetBlock(block, i);
			}
		}

		public void CreateBlocks(int count)
		{
			var size = GInstance.Size;

			for(int i = 0; i < count; ++i)
			{
				var block = new Block(GInstance, new Vector2f(mRand.Next(100, (int)(size.X * 0.25f)), 10));
				block.FillColor = Color.Red;

				ResetBlock(block, i);

				AddChild(block);
				Blocks.Add(block);
			}
		}

		public void ResetBlock(Block block, int heightMult)
		{
			block.Position = new Vector2f(mRand.Next(0, (int)GInstance.Size.X),
				-Difficulty.BlockSpacing * heightMult + mRand.Next(-15, 15));

			CreateBlockUpgrades(block);

			block.Scored = false;
		}

		public void CreateBlockUpgrades(Block block)
		{
			block.Children.Clear();

			if(mRand.Next(UpgradeChance) == 1)
			{
				var upgrade = new Pickup(GInstance);

				upgrade.Position = new Vector2f(
					mRand.Next(-25, (int)block.Shape.Size.X + 25),
					mRand.Next(2) == 1 ?
					upgrade.Shape.Size.Y + mRand.Next(5, 30) : -mRand.Next(5, 30));

				block.AddChild(upgrade);
			}
		}

		public void Reset()
		{
			Blocks.Clear();
			Children.Clear();

			mMaxBlocks = 0;
		}
	}
}