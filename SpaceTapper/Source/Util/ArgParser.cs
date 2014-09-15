using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SpaceTapper
{
	public class ArgParser
	{
		public Dictionary<string, Action<string>> Callbacks;

		public ArgParser()
		{
			Callbacks = new Dictionary<string, Action<string>>();
		}

		public void Add(string name, Action<string> func)
		{
			Callbacks.Add(name, func);
		}

		public void Parse(string[] args)
		{
			foreach(var arg in args)
			{
				var match = Regex.Match(arg, @"--?(\w+)(\s*=\s*(.+))?");

				if(!match.Success)
				{
					Console.WriteLine("Malformed cfg argument: " + arg);
					continue;
				}

				var name = match.Groups[1].Captures[0].Value;

				if(!Callbacks.ContainsKey(name))
				{
					Console.WriteLine("Unknown cfg argument: " + name);
					continue;
				}

				Callbacks[name].Invoke(match.Groups[match.Groups.Count - 1].Value);
			}
		}
	}
}