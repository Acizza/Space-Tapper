using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Entities;
using SpaceTapper.Scenes;
using SpaceTapper.Util;

namespace SpaceTapper
{
	[GameScene("game")]
	public class GameScene : Scene
	{
		public bool InProgress { get; private set; }

		public Player Player             { get; private set; }
		public BlockSpawner BlockSpawner { get; private set; }

		public GameScene(Game game) : base(game)
		{
			Player       = new Player(this, new Vector2f(game.Window.Size.X / 2, game.Window.Size.Y / 2));
			BlockSpawner = new BlockSpawner(this);

			BlockSpawner.Collision += (obj) => OnPlayerCollision();
			BlockSpawner.AddBlocks();

			Entities = new List<Entity>
			{
				Player,
				BlockSpawner
			};

			Input.Keys.AddOrUpdate(Keyboard.Key.Escape, p => Game.SetActiveScene("menu"));
		}

		#region Public methods

		/// <summary>
		/// Starts a new game if one is not already in progress.
		/// </summary>
		public void StartNewGame()
		{
			EndGame();

			foreach(var entity in Entities)
				entity.Reset();

			InProgress = true;
		}

		/// <summary>
		/// Ends the game if one is in progress.
		/// </summary>
		public void EndGame()
		{
			if(!InProgress)
				return;

			InProgress = false;
		}

		public override void Enter()
		{
			if(!InProgress)
				StartNewGame();
		}

		public override void Leave()
		{

		}

		public override void Update(GameTime time)
		{
			base.Update(time);

			foreach(var entity in Entities)
				entity.Update(time);
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			foreach(var entity in Entities)
				target.Draw(entity, states);
		}

		#endregion
		#region Private methods

		void OnPlayerCollision()
		{
			EndGame();
			Game.SetActiveScene("end_game");
		}

		#endregion
	}
}