using System;
using SFML.Window;
using SpaceTapper.Util;

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

			Log.Info("Parsing arguments");

			var parser = new CommandParser();

			parser["w|width"]         = new CommandInfo(false, v => settings.Mode.Width  = uint.Parse(v));
			parser["h|height"]        = new CommandInfo(false, v => settings.Mode.Height = uint.Parse(v));
			parser["v|vsync"]         = new CommandInfo(false, v => settings.Vsync = bool.Parse(v));
			parser["a|autosize"]      = new CommandInfo(true,  v => settings.Mode = VideoMode.DesktopMode);
			parser["f|fullscreen"]    = new CommandInfo(true,  v => settings.Style = Styles.Fullscreen);
			parser["minorVersion"]    = new CommandInfo(false, v => settings.CSettings.MinorVersion = uint.Parse(v));
			parser["majorVersion"]    = new CommandInfo(false, v => settings.CSettings.MajorVersion = uint.Parse(v));
			parser["aa|antialiasing"] = new CommandInfo(false, v => settings.CSettings.AntialiasingLevel = uint.Parse(v));

			parser.Parse(args);

			Log.Info("Finished parsing arguments");

			var game = new Game(settings);
			game.Run();
		}
	}
}