using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.UI;

namespace SpaceTapper.States
{
	[StateAttr]
	public class MenuState : State
	{
		Text 	   _titleText;
		ButtonList _buttons;

		public MenuState()
		{
			base.Name = "menu";

			Input.Keys[Keyboard.Key.Escape] = OnEscapePressed;

			PopulateDrawables();
		}

		void PopulateDrawables()
		{
			var font  = Game.Fonts["default"];
			var hSize = Game.Size / 2;

			_titleText          = new Text(Program.Name, font, 40);
			_titleText.Origin   = new Vector2f(_titleText.GetLocalBounds().Width / 2, 0);
			_titleText.Position = new Vector2f(hSize.X, (int)(Game.Window.Size.Y * 0.2f));

			_buttons = new ButtonList(this);

			var startBtn      = new Button(this);
			startBtn.Text     = new Text("Start", font, 22);
			startBtn.Position = new Vector2f(hSize.X, hSize.Y);
			startBtn.Pressed  += OnStartPressed;

			startBtn.Center();
			_buttons.Add(startBtn);

			var quitBtn      = new Button(this);
			quitBtn.Text     = new Text("Quit", font, 22);
			quitBtn.Position = new Vector2f(hSize.X, hSize.Y + 25);
			quitBtn.Pressed  += OnQuitPressed;

			quitBtn.Center();
			_buttons.Add(quitBtn);
		}

		static void OnEscapePressed(bool pressed)
		{
			if(pressed)
				return;

			Game.Exit();
		}

		public override void Update(double dt)
		{
		}

		public override void Draw(RenderTarget target)
		{
			target.Draw(_titleText);

			foreach(var button in _buttons)
				target.Draw(button);
		}

		static void OnStartPressed()
		{
			Game.SetActiveState("difficulty_select");
		}

		static void OnQuitPressed()
		{
			Game.Exit();
		}
	}
}