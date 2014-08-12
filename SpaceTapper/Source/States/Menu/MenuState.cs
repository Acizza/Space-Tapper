using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class MenuState : AState
	{
		public Button StartButton { get; private set; }
		public Button QuitButton { get; private set; }

		public MenuState(Game instance, bool active = true) : base(instance, active)
		{
			var center = instance.Size / 2;

			StartButton = new Button(instance, center, "Start");
			QuitButton  = new Button(instance,
							center + new Vector2f(0, StartButton.LocalBounds.Height + 15), "Quit");

			StartButton.OnPressed += () => GInstance.GetState<GameState>(State.Game).StartNewGame();
			QuitButton.OnPressed += () => GInstance.Window.Close();

			GInstance.GetState<GameState>(State.Game).OnStartGame += () => Active = false;
		}

		public override void Update(TimeSpan dt)
		{
			StartButton.Update(dt);
			QuitButton.Update(dt);
		}

		public override void Draw(RenderWindow window)
		{
			window.Draw(StartButton);
			window.Draw(QuitButton);
		}

		protected override void OnKeyPressed(KeyEventArgs e)
		{
			switch(e.Code)
			{
				case Keyboard.Key.Escape:
					GInstance.Window.Close();
					break;

				case Keyboard.Key.Return:
					GInstance.GetState<GameState>(State.Game).StartNewGame();
					break;
			}
		}
	}
}