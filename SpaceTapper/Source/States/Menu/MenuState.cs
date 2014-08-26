using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class MenuState : AIdleState
	{
		public ButtonList Buttons { get; private set; }
		public Button StartButton { get; private set; }
		public Button QuitButton { get; private set; }
		public Button ResumeButton { get; private set; }

		GameState mGState;

		public MenuState(Game instance, bool active = true) : base(instance, active)
		{
			CreateButtons();

			mGState = GInstance.GetState<GameState>();

			OnKeyPressed += KeyPressedHandler;
		}

		public override void Update(TimeSpan dt)
		{
			Buttons.MinIndex = mGState.InGame ? 0 : 1;

			if(mGState.InGame)
				ResumeButton.Update(dt);

			StartButton.Update(dt);
			QuitButton.Update(dt);
		}

		public override void Draw(RenderWindow window)
		{
			base.Draw(window);

			if(mGState.InGame)
				window.Draw(ResumeButton);

			window.Draw(StartButton);
			window.Draw(QuitButton);
		}

		void KeyPressedHandler(KeyEventArgs e)
		{
			switch(e.Code)
			{
				case Keyboard.Key.Escape:
					if(mGState.InGame)
						GInstance.SetActiveState(State.Game);
					else
						GInstance.Window.Close();

					break;
			}
		}

		void CreateButtons()
		{
			StartButton = new Button(this, GInstance.Size / 2, "Start");

			var center = StartButton.Text.Position;
			var offset = new Vector2f(0, StartButton.LocalBounds.Height + 15);

			ResumeButton = new Button(this, center - offset, "Resume");
			QuitButton   = new Button(this, center + offset, "Quit");

			StartButton.OnPressed  += () => GInstance.OnEndFrame += OnStartPressed;
			ResumeButton.OnPressed += () => OnResumePressed();
			QuitButton.OnPressed   += () => GInstance.Window.Close();

			Buttons = new ButtonList(this, ResumeButton, StartButton, QuitButton);
			Buttons.MinIndex = 1;
		}

		void OnStartPressed()
		{
			GInstance.SetActiveState(State.DifficultySelect);
			GInstance.SetStateStatus(State.Game, false, true);

			GInstance.OnEndFrame -= OnStartPressed;
		}

		void OnResumePressed()
		{
			GInstance.SetActiveState(State.Game);
		}
	}
}