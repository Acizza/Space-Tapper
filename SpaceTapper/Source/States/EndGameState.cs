using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.States;
using SpaceTapper.UI;
using SpaceTapper.Util;

namespace SpaceTapper
{
	[StateAttribute]
	public class EndGameState : ForegroundState
	{
		Text _scoreText;
		ButtonList _buttons;

		public EndGameState()
		{
			base.Name = "end_game";

			_scoreText = new Text("", Game.Fonts["default"], 40);
			_buttons   = new ButtonList(this);

			var scoreBtn = new Button(this, "Scores", Game.Fonts["default"], (Game.Size / 2).ToFloat());
			var menuBtn  = new Button(this, "Menu",
				Game.Fonts["default"],
				(Game.Size / 2).ToFloat() + new Vector2f(0, 30));

			scoreBtn.Pressed += () => State.SetActive("scores", "game", false, true);
			menuBtn.Pressed  += () => State.SetActive("menu", "game", false, true);

			_buttons.Add(scoreBtn);
			_buttons.Add(menuBtn);

			Input.Keys[Keyboard.Key.Escape] = p =>
			{
				if(!p)
					return;

				State.SetActive("menu", "game", false, true);
			};
		}

		public override void Enter()
		{
			uint score = State.Get<GameState>("game").Score;

			_scoreText.DisplayedString = "Score: " + score;

			_scoreText.Position = new Vector2f(
				Game.Size.X / 2 - _scoreText.GetLocalBounds().Width / 2,
				Game.Size.Y * 0.225f);

			State.Get<ScoresState>("scores").AddScore(score);
		}

		public override void Update(float dt)
		{
			foreach(var btn in _buttons)
				btn.Update(dt);
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			base.Draw(target, states);

			target.Draw(_scoreText, states);

			foreach(var btn in _buttons)
				target.Draw(btn, states);
		}
	}
}