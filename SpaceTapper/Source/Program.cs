using System;
using System.IO;
using SFML.Window;
using SpaceTapper.Util;

namespace SpaceTapper
{
	public class Program
	{
		public const string Name = "Space Tapper";
		public const uint DefaultWidth   = 1024;
		public const uint DefaultHeight  = 768;
		public const string DefaultState = "menu";

		static void Main(string[] args)
		{
			var settings = new GameSettings();

			settings.Mode   = new VideoMode(DefaultWidth, DefaultHeight);
			settings.Vsync  = true;
			settings.Title  = Name;

			var parser = new CommandParser();

			parser["w|width"]      = new CommandInfo(v => settings.Mode.Width  = uint.Parse(v));
			parser["h|height"]     = new CommandInfo(v => settings.Mode.Height = uint.Parse(v));
			parser["v|vsync"]      = new CommandInfo(v => settings.Vsync       = bool.Parse(v));
			parser["f|fullscreen"] = new CommandInfo(v => settings.Fullscreen  = bool.Parse(v));
			parser["a|autosize"]   = new CommandInfo(v => settings.Mode = VideoMode.DesktopMode, true);
			parser["file|parse"]   = new CommandInfo(v => parser.Parse(File.ReadAllLines(v)));
			parser["log"]          = new CommandInfo(v => Log.File = v);

			parser.Parse(args);

			Game.Init(settings);
			Game.DefaultState = Game.GetState(DefaultState);
			Game.Run();
		}
	}
}