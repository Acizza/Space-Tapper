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
			CreateButtons();

			var gState = GInstance.GetState<GameState>();

			gState.OnStartGame += () => mInGame = true;
			gState.OnEndGame   += () => mInGame = false;
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
					if(mInGame)
						GInstance.SetActiveState(State.Game);
					else
						GInstance.Window.Close();

					break;

				case Keyboard.Key.Return:
					OnStartPressed();
					break;
			}
		}

		void CreateButtons()
		{
			StartButton = new Button(GInstance, GInstance.Size / 2, "Start");

			var center = StartButton.Text.Position;
			var offset = new Vector2f(0, StartButton.LocalBounds.Height + 15);

			ResumeButton = new Button(GInstance, center - offset, "Resume");
			QuitButton   = new Button(GInstance, center + offset, "Quit");

			StartButton.OnPressed  += () => GInstance.OnEndFrame += OnStartPressed;
			ResumeButton.OnPressed += () => OnResumePressed();
			QuitButton.OnPressed   += () => GInstance.Window.Close();
		}

		void OnStartPressed()
		{
			GInstance.SetActiveState(State.DifficultySelect);
			Active = false;

			GInstance.OnEndFrame -= OnStartPressed;
		}

		void OnResumePressed()
		{
			GInstance.SetActiveState(State.Game);
			GInstance.GetState<GameState>().Resume();
		}
	}
}