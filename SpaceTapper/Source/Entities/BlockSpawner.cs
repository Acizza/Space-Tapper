using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Physics;
using SpaceTapper.Scenes;
using SpaceTapper.Settings;

namespace SpaceTapper.Entities
{
	public sealed class BlockSpawner : Entity, ICollidable
	{
		public List<Block> Blocks;

		public event Action<Entity> Collision = delegate {};

		public BlockSpawner(Scene scene) : base(scene)
		{
			Blocks = new List<Block>();
			Difficulty.Changed += DifficultyChangeHandler;
		}

		~BlockSpawner()
		{
			Difficulty.Changed -= DifficultyChangeHandler;
		}

		void DifficultyChangeHandler(Difficulty.Settings pSettings, Difficulty.Settings cSettings)
		{
			int blockDiff = Math.Abs(pSettings.BlockCount - cSettings.BlockCount);

			// More blocks in new settings
			if(blockDiff > 0)
			{
				for(int i = 0; i < blockDiff; ++i)
					AddBlock();
			}
			else
			{
				Blocks.RemoveRange(Blocks.Count - blockDiff, blockDiff);
			}

			// Generate a new postion for every block and update spacings
			Reset();
		}

		/// <summary>
		/// Adds a new block to the spawner.
		/// </summary>
		public void AddBlock()
		{
			var size = new Vector2f(Game.Random.Next(75, (int)(Scene.Game.Window.Size.X * 0.45f)),
				           Game.Random.Next(10, 15));

			var block = new Block(size);
			block.FillColor = Color.Red;
			block.Position  = GenerateBlockPosition(Blocks.Count);

			Blocks.Add(block);
		}

		/// <summary>
		/// Adds the amount of blocks specified by the current difficulty level.
		/// </summary>
		public void AddBlocks()
		{
			for(int i = 0; i < Difficulty.Level.BlockCount; ++i)
				AddBlock();
		}

		/// <summary>
		/// Generates a random block position.
		/// </summary>
		/// <returns>The generated block position.</returns>
		/// <param name="index">A multiplier to use for height offsets.</param>
		public Vector2f GenerateBlockPosition(int index)
		{
			float x = Game.Random.Next(-5, (int)(Scene.Game.Window.Size.X * 0.9f));
			float y = -Difficulty.Level.BlockSpacing * index + Game.Random.Next(-30, 30);

			return new Vector2f(x, y);
		}

		/// <summary>
		/// Resets the block and generates a new position for it.
		/// </summary>
		/// <param name="block">Block to reset.</param>
		/// <param name="index">A multiplier to use for height offsets.</param>
		public void ResetBlock(Block block, int index)
		{
			block.Reset();
			block.Position = GenerateBlockPosition(index);
		}

		public override void Reset()
		{
			for(int i = 0; i < Blocks.Count; ++i)
				ResetBlock(Blocks[i], i);
		}

		public bool Collides(Entity entity)
		{
			return Blocks.Any(x => x.GetGlobalBounds().Intersects(entity.GlobalBounds));
		}

		public void OnCollision(Entity entity, Collider.Side side)
		{
			Collision.Invoke(entity);
		}

		public override void Update(GameTime time)
		{
			for(int i = 0; i < Blocks.Count; ++i)
			{
				var block = Blocks[i];

				block.Position += new Vector2f(0, Difficulty.Level.BlockSpeed * time.DeltaTime);

				if(block.Position.Y > Scene.Game.Window.Size.Y)
					ResetBlock(block, i);
			}
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;

			foreach(var block in Blocks)
				target.Draw(block, states);
		}
	}
}