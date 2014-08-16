using System;
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;

namespace SpaceTapper
{
	public class DifficultyState : AState
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
			var pos = GInstance.Size / 2f - new Vector2f(0, (levels.Length * spacing));

			int i = 0;

			foreach(var difficulty in levels)
			{
				++i;
				int copy = i; // http://stackoverflow.com/questions/271440/captured-variable-in-a-loop-in-c-sharp

				var button = new Button(GInstance, new Vector2f(pos.X, pos.Y + spacing * i), difficulty);

				button.OnPressed += () =>
				{
					GInstance.GetState<GameState>(State.Game).StartNewGame((DifficultyLevel)(copy - 1));
				};

				mButtons.Add(button);
			}
		}

		void EndFrameHandler()
		{
			GInstance.SetActiveState(State.Menu);
			GInstance.OnEndFrame -= EndFrameHandler;
		}
	}

	public enum DifficultyLevel
	{
		Easy,
		Normal,
		Hard,
		Hell
	}

	public struct DifficultySettings
	{
		public float BlockSpeed;
		public float BlockSpacing;
		public int BlockCount;

		public DifficultySettings(float blockSpeed, int blockCount, float blockSpacing)
		{
			BlockSpeed = blockSpeed;
			BlockCount = blockCount;
			BlockSpacing = blockSpacing;
		}
	}

	public static class Difficulty
	{
		public static Dictionary<DifficultyLevel, DifficultySettings> Levels { get; private set; }

		static Difficulty()
		{
			Levels = new Dictionary<DifficultyLevel, DifficultySettings>();

			Levels[DifficultyLevel.Easy] = new DifficultySettings(
				blockSpeed: 95, blockCount: 80, blockSpacing: 160
			);

			Levels[DifficultyLevel.Normal] = new DifficultySettings(
				blockSpeed: 150, blockCount: 100, blockSpacing: 125
			);

			Levels[DifficultyLevel.Hard] = new DifficultySettings(
				blockSpeed: 175, blockCount: 200, blockSpacing: 110
			);

			Levels[DifficultyLevel.Hell] = new DifficultySettings(
				blockSpeed: 230, blockCount: 400, blockSpacing: 95
			);
		}
	}
}