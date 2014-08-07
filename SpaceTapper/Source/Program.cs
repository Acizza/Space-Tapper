using System;
using SFML.Window;
using System.IO;
using SpaceTapper;

namespace SpaceTapper
{
	public class Program
	{
		public static readonly uint DefaultWidth = 1024;
		public static readonly uint DefaultHeight = 768;
		public static readonly string DefaultTitle = "Space Tapper";
		public static readonly string DefaultConfigFile = "settings.cfg";

		static void Main(string[] args)
		{
			var settings = new GameSettings();

			settings.Mode.Width  = DefaultWidth;
			settings.Mode.Height = DefaultHeight;
			settings.Style       = Styles.Close;
			settings.Title       = DefaultTitle;

			var parser = new ArgParser();

			parser.Add("file",       v => parser.Parse(File.ReadAllLines(v)));
			parser.Add("width",      v => settings.Mode.Width = uint.Parse(v));
			parser.Add("height",     v => settings.Mode.Height = uint.Parse(v));
			parser.Add("vsync",      v => settings.Vsync = bool.Parse(v));
			parser.Add("fullscreen", v => settings.Style = bool.Parse(v) == true ? Styles.Fullscreen : Styles.Close);
			parser.Add("autosize",   v => settings.Mode = VideoMode.DesktopMode);

			if(File.Exists(DefaultConfigFile))
				parser.Parse(File.ReadAllLines(DefaultConfigFile));

			parser.Parse(args);

			var game = new Game(settings);
			game.Run();
		}
	}
}