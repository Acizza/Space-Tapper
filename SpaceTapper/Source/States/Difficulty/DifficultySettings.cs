using System;
using System.Collections.Generic;

namespace SpaceTapper
{
	public enum DifficultyLevel
	{
		Easy,
		Normal,
		Hard,
		Hell
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
}