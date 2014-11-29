using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SpaceTapper.Physics;
using SpaceTapper.Scenes;
using SpaceTapper.Settings;
using SFML.Window;

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

			AddBlocks();
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

			Reset();
		}

		public void AddBlock()
		{
			var size = new Vector2f(Game.Random.Next(75, (int)(Scene.Game.Window.Size.X * 0.45f)),
				           Game.Random.Next(10, 15));

			var block = new Block(size);
			block.FillColor = Color.Red;
			block.Position  = GenerateBlockPosition(Blocks.Count);

			Blocks.Add(block);
		}

		public void AddBlocks()
		{
			for(int i = 0; i < Difficulty.Level.BlockCount; ++i)
				AddBlock();
		}

		public Vector2f GenerateBlockPosition(int index)
		{
			float x = Game.Random.Next(-5, (int)(Scene.Game.Window.Size.X * 0.9f));
			float y = -Difficulty.Level.BlockSpacing * index + Game.Random.Next(-30, 30);

			return new Vector2f(x, y);
		}

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