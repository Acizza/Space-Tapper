using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpaceTapper.Util;

namespace SpaceTapper
{
	/// <summary>
	/// Parses whitespace separated text with regex.
	/// </summary>
	public class CommandParser
	{
		public Dictionary<string, CommandInfo> Callbacks;

		public static char NameSeparator = '|';

		public CommandParser()
		{
			Callbacks = new Dictionary<string, CommandInfo>();
		}

		/// <summary>
		/// Ditto for Add().
		/// </summary>
		/// <param name="name">Command name.</param>
		public CommandInfo this[string name]
		{
			get
			{
				return Callbacks[name];
			}
			set
			{
				Add(name, value.Single, value.Callback);
			}
		}

		/// <summary>
		/// Adds a command to the parser.
		/// </summary>
		/// <param name="name">Name. Can be separated by CommandParser.NameSeparator to use multiple aliases.
		/// Will not overwrite others.</param>
		/// <param name="del">Delegate. String parameter is the value returned when parsing.</param>
		public void Add(string name, bool single, Action<string> del)
		{
			var names = name.Split(NameSeparator);

			foreach(var n in names)
			{
				if(!Callbacks.ContainsKey(n))
					Callbacks[n] = new CommandInfo(del, single);
			}
		}

		public void Parse(string[] args)
		{
			for(int i = 0; i < args.Length; ++i)
			{
				var arg = args[i];

				if(arg[0] != '-')
				{
					Log.Warning("Skipping argument: ", arg);
					continue;
				}

				var name  = arg.Substring(1);

				if(!Callbacks.ContainsKey(name))
				{
					Log.Error("Unknown command: ", arg);
					continue;
				}

				var callback = Callbacks[name];

				// If the command indicates it requires a value, increase i to skip the next argument for name parsing.
				if(!callback.Single && i + 1 < args.Length)
					++i;

				Callbacks[name].Callback.Invoke(args[i]);
			}
		}
	}

	public struct CommandInfo
	{
		public Action<string> Callback;

		/// <summary>
		/// If true, the parser will not look for a value ahead.
		/// </summary>
		public bool Single;

		public CommandInfo(Action<string> callback, bool single = false)
		{
			Callback = callback;
			Single = single;
		}
	}
}