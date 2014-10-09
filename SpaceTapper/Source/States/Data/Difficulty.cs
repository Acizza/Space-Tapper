using System;
using System.Collections.Generic;

namespace SpaceTapper.States.Data
{
	/// <summary>
	/// Provides the default difficulty level values.
	/// </summary>
	public static class Difficulty
	{
		public static Dictionary<GameDifficulty, DifficultySettings> Levels { get; private set; }

		static Difficulty()
		{
			Levels = new Dictionary<GameDifficulty, DifficultySettings>();

			Levels[GameDifficulty.Easy] = new DifficultySettings(
				blockSpeed: 95, blockCount: 80, blockSpacing: 160
			);

			Levels[GameDifficulty.Normal] = new DifficultySettings(
				blockSpeed: 150, blockCount: 100, blockSpacing: 125
			);

			Levels[GameDifficulty.Hard] = new DifficultySettings(
				blockSpeed: 175, blockCount: 200, blockSpacing: 110
			);

			Levels[GameDifficulty.Hell] = new DifficultySettings(
				blockSpeed: 230, blockCount: 400, blockSpacing: 95
			);
		}
	}
}