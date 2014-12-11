using System;
using System.Linq;
using SFML.Window;
using SpaceTapper.Settings;
using System.Collections.Generic;

namespace SpaceTapper
{
	public static class Program
	{
		static void Main(string[] args)
		{
			var settings = new GameSettings
			{
				Mode         = new VideoMode(1024, 768),
				CSettings    = new ContextSettings(0, 0, 0, 3, 3),
				Style        = Styles.Close,
				Vsync        = true,
				KeyRepeat    = true,
				Title        = "Space Tapper",
				DefaultScene = "menu"
			};

			Options.ReadAll();

			// TODO: Come up with better way of parsing options and unifying parameters with options.
			// LINQ query below takes all options and joins them via key and value.
			Parameters.Parse(ref settings, Options.All.Select(x => new List<string> { Parameters.ArgSpecifier + x.Key, x.Value }).SelectMany(x => x).ToArray());
			Parameters.Parse(ref settings, args);

			var game = new Game(settings);
			game.Run();
		}
	}
}