using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.States;
using SpaceTapper.UI;

namespace SpaceTapper
{
	[StateAttr]
	public class MenuState : State
	{
		Text mTitleText;
		ButtonList mOptions;

		public MenuState()
		{
			base.Name = "menu";

			var font  = Game.Fonts["default"];
			var hSize = Game.Size / 2;

			mTitleText          = new Text(Program.Name, font, 40);
			mTitleText.Origin   = new Vector2f(mTitleText.GetLocalBounds().Width / 2, 0);
			mTitleText.Position = new Vector2f(hSize.X, (int)(Game.Window.Size.Y * 0.2f));

			mOptions = new ButtonList(this);

			var startBtn      = new Button(this);
			startBtn.Text     = new Text("Start", font, 22);
			startBtn.Position = new Vector2f(hSize.X, hSize.Y);
			startBtn.Pressed  += OnStartPressed;

			startBtn.Center();
			mOptions.Add(startBtn);

			var quitBtn      = new Button(this);
			quitBtn.Text     = new Text("Quit", font, 22);
			quitBtn.Position = new Vector2f(hSize.X, hSize.Y + 25);
			quitBtn.Pressed  += OnQuitPressed;

			quitBtn.Center();
			mOptions.Add(quitBtn);
		}

		public override void Update(double dt)
		{
		}

		public override void Draw(RenderTarget target)
		{
			target.Draw(mTitleText);

			foreach(var button in mOptions)
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