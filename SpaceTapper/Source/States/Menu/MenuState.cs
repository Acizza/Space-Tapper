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
			StartButton.Pressed += OnStartPressed;

			QuitButton = new Button(instance,
				center + new Vector2f(0, StartButton.LocalBounds.Height + 15), "Quit");

			QuitButton.Pressed += () => GInstance.Window.Close();
		}

		public override void Update(TimeSpan dt)
		{
			if(!Updating)
				return;

			StartButton.Update(dt);
			QuitButton.Update(dt);
		}

		public override void Render(RenderWindow window)
		{
			if(!Drawing)
				return;

			window.Draw(StartButton);
			window.Draw(QuitButton);
		}

		void OnStartPressed()
		{
			Active = false;
			GInstance.GameState.StartNewGame();
		}
	}
}