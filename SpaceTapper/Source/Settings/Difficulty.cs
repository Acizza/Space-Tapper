using System;

namespace SpaceTapper.Settings
{
	public static class Difficulty
	{
		public struct Settings
		{
			public string Name;

			public float BlockSpeed;
			public float BlockSpacing;
			public int   BlockCount;

			public Settings(string name, float blockSpeed, float blockSpacing, int blockCount)
			{
				Name = name;

				BlockSpeed   = blockSpeed;
				BlockSpacing = blockSpacing;
				BlockCount   = blockCount;
			}
		}
	
		/// <summary>
		/// Called when Level is changed. Parameters are the old and new settings, respectively.
		/// </summary>
		public static event Action<Settings, Settings> Changed = delegate {};

		/// <summary>
		/// The current difficulty level.
		/// </summary>
		/// <value>The current difficulty level.</value>
		public static Settings Level { get; private set; }

		/// <summary>
		/// Returns all difficulty levels.
		/// </summary>
		/// <value>The difficulty levels.</value>
		public static Settings[] Levels
		{
			get
			{
				return _levels;
			}
		}

		static readonly Settings[] _levels;

		static Difficulty()
		{
			_levels = new []
			{
				new Settings("Easy",   blockSpeed: 95, blockCount: 75, blockSpacing: 160),
				new Settings("Normal", blockSpeed: 150, blockCount: 75, blockSpacing: 125),
				new Settings("Hard",   blockSpeed: 175, blockCount: 75, blockSpacing: 110),
				new Settings("Hell",   blockSpeed: 215, blockCount: 75, blockSpacing: 100)
			};

			SetLevel(0);
		}

		/// <summary>
		/// Sets the current difficulty level.
		/// </summary>
		/// <param name="name">Difficulty name.</param>
		public static void SetLevel(string name)
		{
			SetLevel(Array.FindIndex(_levels, x => x.Name == name));
		}

		/// <summary>
		/// Sets the current difficulty level, by index.
		/// </summary>
		/// <param name="index">Level index.</param>
		public static void SetLevel(int index)
		{
			Changed.Invoke(Level, _levels[index]);
			Level = _levels[index];
		}
	}
}