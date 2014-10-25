using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.States;
using SpaceTapper.States.Data;

namespace SpaceTapper.Ents
{
	public class BlockSpawner : Entity
	{
		public List<Block> Blocks;

		/// <summary>
		/// Game difficulty. If set, it will automatically create / remove blocks as necessary.
		/// </summary>
		/// <value>The settings.</value>
		public DifficultySettings Settings
		{
			get
			{
				return _settings;
			}
			set
			{
				if(value.BlockCount > _settings.BlockCount)
				{
					for(int i = _settings.BlockCount; i < value.BlockCount; ++i)
						CreateBlock();
				}
				else if(value.BlockCount < _settings.BlockCount)
				{
					var diff = Math.Abs(value.BlockCount - _settings.BlockCount);

					Blocks.RemoveRange(Blocks.Count - diff, diff);

					for(int i = 0; i < Blocks.Count; ++i)
						ResetBlock(i);
				}

				_settings = value;
			}
		}

		DifficultySettings _settings;

		public BlockSpawner(State state) : base(state)
		{
			Blocks = new List<Block>();
		}

		public BlockSpawner(State state, int initialBlocks, DifficultySettings settings)
			: base(state)
		{
			Blocks   = new List<Block>(initialBlocks);
			Settings = settings;
		}

		public void CreateBlock()
		{
			var size  = new Vector2f(Game.Random.Next(75, (int)(Game.Size.X * 0.45f)),
									 Game.Random.Next(10, 15));

			var block = new Block(size);
			block.FillColor = Color.Red;

			Blocks.Add(block);
			ResetBlock(Blocks.Count - 1);
		}

		/// <summary>
		/// Repositions the block found by index and resets its Passed boolean.
		/// </summary>
		/// <param name="index">Index.</param>
		public void ResetBlock(int index)
		{
			var block = Blocks[index];
			var xPos  = Game.Random.Next(-5, (int)(Game.Size.X * 0.9f));
			var yPos  = -Settings.BlockSpacing * index + Game.Random.Next(-30, 30);

			block.Position = new Vector2f(xPos, yPos);
			block.Passed   = false;
		}

		/// <summary>
		/// Calls ResetBlock() on every block in Blocks.
		/// </summary>
		public void Reset()
		{
			for(int i = 0; i < Blocks.Count; ++i)
				ResetBlock(i);
		}

		protected override void UpdateSelf(float dt)
		{
			for(int i = 0; i < Blocks.Count; ++i)
			{
				var block = Blocks[i];

				block.Position += new Vector2f(0, Settings.BlockSpeed * dt);

				if(block.Position.Y >= Game.Size.Y)
					ResetBlock(i);
			}
		}

		protected override void DrawSelf(RenderTarget target, RenderStates states)
		{
			foreach(var block in Blocks)
				target.Draw(block, states);
		}
	}
}