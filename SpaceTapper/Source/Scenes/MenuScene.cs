using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Scenes;
using SpaceTapper.UI;
using SpaceTapper.Util;
using System.Collections.Generic;

namespace SpaceTapper.Scenes
{
	[GameScene("menu")]
	public class MenuScene : Scene
	{
		Text _titleText;

		ButtonList _buttons;
		Button _resumeBtn;

		public MenuScene(Game game) : base(game)
		{
			_titleText = new Text(game.Settings.Title, new Font("Resources/Fonts/DejaVuSans.ttf"), 40);
			_titleText.Color    = Color.White;
			_titleText.Position = new Vector2f(game.Window.Size.X / 2, game.Window.Size.Y * 0.25f).Truncate();
			_titleText.Origin   = _titleText.GetLocalBounds().Size() / 2;

			CreateButtons();

			Input.Keys.AddOrUpdate(Keyboard.Key.Escape, p =>
			{
				if(!p)
					return;

				Game.Exit();
			});
		}

		void CreateButtons()
		{
			var font = new Font("Resources/Fonts/DejaVuSans.ttf");
			var pos  = Game.Window.Size.ToVector2f() / 2;

			const int fSize  = 24;
			const int offset = (int)(fSize * 1.35f);

			_resumeBtn = new Button(this, font, pos, fSize, "Resume");
			_resumeBtn.Pressed += () => Game.SetActiveScene("game");

			pos.Y += offset;

			var startBtn = new Button(this, font, pos, fSize, "Start");
			startBtn.Pressed += () =>
			{
				Game.GetScene<GameScene>("game").StartNewGame();
				Game.SetActiveScene("game");
			};

			pos.Y += offset;

			var optionsBtn = new Button(this, font, pos, fSize, "Options");
			optionsBtn.Pressed += () => Game.SetActiveScene("options");

			pos.Y += offset;

			var quitBtn = new Button(this, font, pos, fSize, "Quit");
			quitBtn.Pressed += Game.Exit;

			_buttons = new ButtonList(Input, _resumeBtn, startBtn, optionsBtn, quitBtn);
		}

		public override void Enter()
		{
			_resumeBtn.Enabled = Game.GetScene<GameScene>("game").InProgress;
		}

		public override void Leave()
		{

		}

		public override void Update(GameTime time)
		{

		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			target.Draw(_titleText, states);

			for(int i = _buttons.MinIndex; i <= _buttons.MaxIndex; ++i)
				target.Draw(_buttons.Buttons[i], states);
		}
	}
}