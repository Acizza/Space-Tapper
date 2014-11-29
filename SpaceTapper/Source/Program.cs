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

			//Parameters.Parse();
			// TODO: Revamp SpaceTapper.Util.CommandParser for the Parameters class.

			Parser = new CommandParser();

			// Currently just going to use a slight hack
			foreach(var param in Parameters.All)
			{
				Parser.Add(
					param.Key.Name,
					!param.Key.ValueNeeded,
					v => param.Value(ref settings, v),
					param.Key.Description);
			}

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