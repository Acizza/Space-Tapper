using System;
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;

namespace SpaceTapper
{
	public class DifficultyState : AIdleState
	{
		ButtonList mButtons;

		public DifficultyState(Game instance, bool active = false) : base(instance, active)
		{
			mButtons = new ButtonList(this);

			InitButtons();

			OnKeyPressed += KeyPressedHandler;
		}

		public override void Update(TimeSpan dt)
		{
			foreach(var button in mButtons.Buttons)
				button.Update(dt);
		}

		public override void Draw(RenderWindow window)
		{
			base.Draw(window);

			foreach(var button in mButtons.Buttons)
				window.Draw(button);
		}

		void KeyPressedHandler(KeyEventArgs e)
		{
			if(e.Code == Keyboard.Key.Escape)
				GInstance.OnEndFrame += OnEscapePressed;
		}

		void InitButtons()
		{
			var spacing = 25;

			var levels = Enum.GetNames(typeof(DifficultyLevel));
			var pos = GInstance.Size / 2f - new Vector2f(0, levels.Length * spacing);

			int i = 0;

			foreach(var difficulty in levels)
			{
				++i;
				int copy = i; // http://stackoverflow.com/questions/271440/captured-variable-in-a-loop-in-c-sharp

				var button = new Button(this, new Vector2f(pos.X, pos.Y + spacing * i), difficulty);

				button.OnPressed += () =>
				{
					Active = false;
					GInstance.GetState<GameState>().StartNewGame((DifficultyLevel)(copy - 1));
				};

				mButtons.Add(button);
			}
		}

		void OnEscapePressed()
		{
			GInstance.SetActiveState(State.Menu);
			GInstance.SetStateStatus(State.Game, false, true);

			GInstance.OnEndFrame -= OnEscapePressed;
		}
	}
}