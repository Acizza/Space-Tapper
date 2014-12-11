using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SpaceTapper.Util;

namespace SpaceTapper
{
	public static class Options
	{
		public const string OptionsFile = "settings.cfg";

		public static Dictionary<string, string> All;

		static Options()
		{
			All = new Dictionary<string, string>();
		}

		/// <summary>
		/// Reads all settings from the file specified by OptionsFile.
		/// </summary>
		public static void ReadAll()
		{
			if(!File.Exists(OptionsFile))
				File.WriteAllText(OptionsFile, "");

			foreach(var line in File.ReadAllLines(OptionsFile))
			{
				var subText = line.Split(' ');

				if(subText.Length < 2)
				{
					Log.Warning("Malformed option in " + OptionsFile + ": " + line);
					continue;
				}

				All[subText[0]] = String.Join(" ", subText.Skip(1));
			}
		}

		/// <summary>
		/// Writes all settings to the file specified by OptionsFile.
		/// </summary>
		public static void WriteAll()
		{
			File.WriteAllLines(OptionsFile, All.Select(x => x.Key + " " + x.Value));
		}
	}
}