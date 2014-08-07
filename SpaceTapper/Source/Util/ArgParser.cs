using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SpaceTapper
{
	public class ArgParser
	{
		public Dictionary<string,CallbackFunc> Callbacks { get; private set; }
		public delegate void CallbackFunc(string value);

		public ArgParser()
		{
			Callbacks = new Dictionary<string,CallbackFunc>();
		}

		public void Add(string name, CallbackFunc func)
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

				// TODO: Refactor single argument matching (?)
				int gIndex = 2;

				if(match.Groups.Count > 3)
					gIndex = 3;

				var mGroup = match.Groups[gIndex];

				if(mGroup.Captures.Count <= 0)
				{
					Callbacks[name].Invoke("");
					continue;
				}

				Callbacks[name].Invoke(mGroup.Captures[0].Value);
			}
		}
	}
}