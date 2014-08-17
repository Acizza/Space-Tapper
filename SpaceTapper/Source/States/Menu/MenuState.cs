using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class MenuState : AIdleState
	{
		public Button StartButton { get; private set; }
		public Button QuitButton { get; private set; }
		public Button ResumeButton { get; private set; }

		bool mInGame;

		public MenuState(Game instance, bool active = true) : base(instance, active)
		{
			StartButton = new Button(instance, GInstance.Size / 2, "Start");

			var lBounds = StartButton.LocalBounds;
			var center = StartButton.Text.Position;

			ResumeButton = new Button(instance,
				center - new Vector2f(0, lBounds.Height + 15), "Resume");

			QuitButton  = new Button(instance,
				center + new Vector2f(0, lBounds.Height + 15), "Quit");

			StartButton.OnPressed += () => GInstance.OnEndFrame += OnStartPressed;
			ResumeButton.OnPressed += () => OnResumePressed();
			QuitButton.OnPressed += () => GInstance.Window.Close();

			var gState = GInstance.GetState<GameState>(State.Game);

			gState.OnStartGame += () => mInGame = true;
			gState.OnEndGame += () => mInGame = false;
		}

		public override void Update(TimeSpan dt)
		{
			if(mInGame)
				ResumeButton.Update(dt);

			StartButton.Update(dt);
			QuitButton.Update(dt);
		}

		public override void Draw(RenderWindow window)
		{
			base.Draw(window);

			if(mInGame)
				window.Draw(ResumeButton);

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
					OnStartPressed();
					break;
			}
		}

		void OnStartPressed()
		{
			GInstance.SetActiveState(State.DifficultySelect);
			GInstance.SetStateStatus(State.Game, false, true);

			GInstance.OnEndFrame -= OnStartPressed;
			Active = false;
		}

		void OnResumePressed()
		{
			GInstance.SetActiveState(State.Game);
			GInstance.GetState<GameState>(State.Game).Resume();
		}
	}
}