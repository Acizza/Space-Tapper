using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Util;

namespace SpaceTapper.Scenes
{
	[GameScene("end_game")]
	public sealed class EndGameScene : Scene
	{
		public EndGameScene(Game game) : base(game)
		{
			Input.Keys.AddOrUpdate(Keyboard.Key.Escape, OnEscapePressed);
		}

		void OnEscapePressed(bool pressed)
		{
			if(!pressed)
				return;

			Game.SetActiveScene("menu");
		}

		public override void Enter()
		{

		}

		public override void Leave()
		{

		}

		public override void Update(GameTime time)
		{

		}

		public override void Draw(RenderTarget target, RenderStates states)
		{

		}
	}
}