using System;
using SpaceTapper.Util;

namespace SpaceTapper
{
	public class Program
	{
		static void Main(string[] args)
		{
			var settings = new GameSettings();
			settings.Width = 1024;
			settings.Height = 768;
			settings.Title = "SpaceTapper";
			settings.Vsync = true;

			Log.File = "./log.txt";

			Game.Init(settings);
			Game.Run();
		}
	}
}