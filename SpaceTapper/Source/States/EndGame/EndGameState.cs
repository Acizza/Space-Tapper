using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class EndGameState : AState
	{
		public static Font Font;
		public Text ScoreText { get; private set; }
		public Text TimeText { get; private set; }

		static EndGameState()
		{
			Font = new Font("data/fonts/DejaVuSans.ttf");
		}

		public EndGameState(Game instance, bool active = false) : base(instance, active)
		{
			TimeText  = new Text("Time: 00:00", Font, 30);
			ScoreText = new Text("Score: 0", Font, 30);

			TimeText.Position = GInstance.Size / 2 - new Vector2f(100, 150);
			ScoreText.Position = GInstance.Size / 2 + new Vector2f(-100, -100);

			GInstance.GetState<GameState>(State.Game).OnEndGame += OnEndGame;
		}

		public override void Update(TimeSpan dt)
		{
		}

		public override void Draw(RenderWindow window)
		{
			if(!Drawing)
				return;
				
			window.Draw(ScoreText);
			window.Draw(TimeText);
		}

		void OnEndGame()
		{
			var gState = GInstance.GetState<GameState>(State.Game);

			TimeText.DisplayedString = gState.TimeText.DisplayedString;
			ScoreText.DisplayedString = "Score: " + gState.Score;

			Active = true;
		}

		protected override void OnKeyPressed(KeyEventArgs e)
		{
			// BUG: Will end up exiting the game since the menu key press is ran right after this.
			if(e.Code == Keyboard.Key.Escape)
				GInstance.SetActiveState(State.Menu);
		}
	}
}