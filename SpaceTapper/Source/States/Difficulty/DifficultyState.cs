using System;
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;

namespace SpaceTapper
{
	public class DifficultyState : AIdleState
	{
		List<Button> mButtons;

		public DifficultyState(Game instance, bool active = false) : base(instance, active)
		{
			mButtons = new List<Button>();

			InitButtons();
		}

		public override void Update(TimeSpan dt)
		{
			foreach(var button in mButtons)
				button.Update(dt);
		}

		public override void Draw(RenderWindow window)
		{
			base.Draw(window);

			foreach(var button in mButtons)
				window.Draw(button);
		}

		protected override void OnKeyPressed(KeyEventArgs e)
		{
			if(e.Code == Keyboard.Key.Escape)
				GInstance.OnEndFrame += EndFrameHandler;
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

				var button = new Button(GInstance, new Vector2f(pos.X, pos.Y + spacing * i), difficulty);

				button.OnPressed += () =>
				{
					Active = false;
					GInstance.GetState<GameState>(State.Game).StartNewGame((DifficultyLevel)(copy - 1));
				};

				mButtons.Add(button);
			}
		}

		void EndFrameHandler()
		{
			GInstance.SetActiveState(State.Menu);
			GInstance.SetStateStatus(State.Game, false, true);

			GInstance.OnEndFrame -= EndFrameHandler;
		}
	}
}