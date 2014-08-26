using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class EndGameState : AIdleState
	{
		public Text ScoreText { get; private set; }
		public Text TimeText { get; private set; }
		public ButtonList Buttons { get; private set; }

		public EndGameState(Game instance, bool active = false) : base(instance, active)
		{
			var font = GInstance.Fonts["default"];

			TimeText  = new Text("Time: 00:00", font, 30);
			ScoreText = new Text("Score: 0", font, 30);

			TimeText.Position = GInstance.Size / 2 - new Vector2f(100, 250);
			ScoreText.Position = GInstance.Size / 2 + new Vector2f(-100, -200);

			GInstance.GetState<GameState>().OnEndGame += OnEndGame;

			InitButtons();

			base.OnKeyPressed += KeyPressedHandler;
		}

		public override void Update(TimeSpan dt)
		{
			foreach(var btn in Buttons.Buttons)
				btn.Update(dt);
		}

		public override void Draw(RenderWindow window)
		{
			base.Draw(window);

			foreach(var btn in Buttons.Buttons)
				window.Draw(btn);

			window.Draw(ScoreText);
			window.Draw(TimeText);
		}

		void InitButtons()
		{
			var scoreBtn = new Button(this, GInstance.Size / 2, "Scores", 26);
			var menuBtn  = new Button(this, GInstance.Size / 2 + new Vector2f(0, 35), "Menu", 26);

			scoreBtn.OnPressed += OnStartBtnPressed;
			menuBtn.OnPressed += () => { GInstance.OnEndFrame += ReturnToMenu; };

			Buttons = new ButtonList(this, scoreBtn, menuBtn);
		}

		void OnEndGame()
		{
			var gState = GInstance.GetState<GameState>();

			TimeText.DisplayedString = gState.TimeText.DisplayedString;
			ScoreText.DisplayedString = gState.ScoreText.DisplayedString;

			GInstance.SetActiveState(this);
			GInstance.SetStateStatus(State.Game, false, true);
		}

		void KeyPressedHandler(KeyEventArgs e)
		{
			if(e.Code == Keyboard.Key.Escape)
				GInstance.OnEndFrame += ReturnToMenu;
		}

		void ReturnToMenu()
		{
			GInstance.SetActiveState(State.Menu);
			GInstance.SetStateStatus(State.Game, false, true);

			GInstance.OnEndFrame -= ReturnToMenu;
		}

		void OnStartBtnPressed()
		{
			GInstance.SetActiveState(State.Scoreboard);
			GInstance.SetStateStatus(State.Game, false, true);
		}
	}
}