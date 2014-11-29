using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Entities;
using SpaceTapper.Scenes;

namespace SpaceTapper
{
	[GameScene("game")]
	public class GameScene : Scene
	{
		public bool InProgress { get; private set; }

		public GameScene(Game game) : base(game)
		{
			Entities = new List<Entity>
			{
				new Player(this, new Vector2f(game.Window.Size.X / 2, game.Window.Size.Y / 2)),
				new BlockSpawner(this)
			};
		}

		public void StartNewGame()
		{
			if(InProgress)
				return;

			foreach(var entity in Entities)
				entity.Reset();

			InProgress = true;
		}

		public void EndGame()
		{
			if(!InProgress)
				return;

			InProgress = false;
		}

		public override void Enter()
		{
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
	}
}