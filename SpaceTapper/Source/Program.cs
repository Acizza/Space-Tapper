using System;
using SFML.Window;
using SpaceTapper.Util;
using System.IO;

namespace SpaceTapper
{
	public static class Program
	{
		public static CommandParser Parser { get; private set; }

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

			Parser = new CommandParser();

			Parser["w|width"] = new CommandInfo(false, v => settings.Mode.Width  = uint.Parse(v),
				"The width of the window.");

			Parser["h|height"] = new CommandInfo(false, v => settings.Mode.Height = uint.Parse(v),
				"The height of the window.");

			Parser["v|vsync"] = new CommandInfo(false, v => settings.Vsync = bool.Parse(v),
				"A true or false value indicating if vertical sync should be enabled.");

			Parser["a|autosize"] = new CommandInfo(true, v => settings.Mode = VideoMode.DesktopMode,
				"Sizes the window to the primary monitor's resolution. Must be after and width / height commands.");

			Parser["f|fullscreen"] = new CommandInfo(true, v => settings.Style = Styles.Fullscreen,
				"Sets the window to fullscreen mode.");

			Parser["minorVersion"] = new CommandInfo(false, v => settings.CSettings.MinorVersion = uint.Parse(v),
				"The minor OpenGL version to use.");

			Parser["majorVersion"] = new CommandInfo(false, v => settings.CSettings.MajorVersion = uint.Parse(v),
				"The major OpenGL version to use.");

			Parser["aa|antialiasing"] = new CommandInfo(false, v => settings.CSettings.AntialiasingLevel = uint.Parse(v),
				"The anti-aliasing level to use for the window.");

			Parser["file|parse"] = new CommandInfo(false, v => Parser.Parse(File.ReadAllLines(v)),
				"The file to parse for additional commands.");

			Parser["log"] = new CommandInfo(false, v => Log.LogFile = v,
				"The file to write all log information to.");

			Parser["help|commands"] = new CommandInfo(true, v => PrintHelp(true),
				"Prints this.");

			Parser.Parse(args);

			var game = new Game(settings);
			game.Run();
		}

		/// <summary>
		/// Prints all commands defined in Parser.
		/// </summary>
		/// <param name="exit">If set to <c>true</c>, the application exits with code 0.</param>
		public static void PrintHelp(bool exit = false)
		{
			Console.WriteLine("Specify command values by placing a space after typing the command.");
			Console.WriteLine("Commands separated by \"{0}\" do the same thing.\n", CommandParser.NameSeparator);

			foreach(var option in Parser.Callbacks.DistinctBy(x => x.Value.FullName))
			{
				Console.WriteLine("{0}{1}:\n\tDescription: {2}\n\tValue needed: {3}\n",
					CommandParser.ArgSpecifier,
					option.Value.FullName,
					option.Value.Description,
					option.Value.NameOnly ? "no" : "yes");
			}

			if(exit)
				Environment.Exit(0);
		}
	}
}