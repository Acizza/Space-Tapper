using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class EndGameState : AState
	{
		public Text ScoreText { get; private set; }
		public Text TimeText { get; private set; }

		public EndGameState(Game instance, bool active = false) : base(instance, active)
		{
			var font = GInstance.Fonts["default"];

			TimeText  = new Text("Time: 00:00", font, 30);
			ScoreText = new Text("Score: 0", font, 30);

			TimeText.Position = GInstance.Size / 2 - new Vector2f(100, 150);
			ScoreText.Position = GInstance.Size / 2 + new Vector2f(-100, -100);

			GInstance.GetState<GameState>(State.Game).OnEndGame += OnEndGame;
		}

		public override void Update(TimeSpan dt)
		{
		}

		public override void Draw(RenderWindow window)
		{
			window.Draw(ScoreText);
			window.Draw(TimeText);
		}

		void OnEndGame()
		{
			var gState = GInstance.GetState<GameState>(State.Game);

			TimeText.DisplayedString = gState.TimeText.DisplayedString;
			ScoreText.DisplayedString = gState.ScoreText.DisplayedString;

			Active = true;
		}

		protected override void OnKeyPressed(KeyEventArgs e)
		{
			if(e.Code == Keyboard.Key.Escape)
				GInstance.OnEndFrame += EndFrameHandler;
		}

		void EndFrameHandler()
		{
			GInstance.SetActiveState(State.Menu);
			GInstance.OnEndFrame -= EndFrameHandler;
		}
	}
}