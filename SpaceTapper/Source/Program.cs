using System;
using SFML.Window;
using SpaceTapper.Settings;

namespace SpaceTapper
{
	public static class Program
	{
		static void Main(string[] args)
		{
			var settings = new GameSettings
			{
				Mode      = new VideoMode(1024, 768),
				CSettings = new ContextSettings(0, 0, 0, 3, 3),
				Style     = Styles.Close,
				Vsync     = true,
				KeyRepeat = false,
				Title     = "Space Tapper"
			};

			Parameters.Parse(ref settings, args);

			var game = new Game(settings);
			game.Run();
		}
	}
}