using System;
using System.Collections.Generic;
using SpaceTapper.Util;

namespace SpaceTapper.Util
{
	public sealed class CommandParser
	{
		/// <summary>
		/// The separator to use for command names.
		/// </summary>
		public const char NameSeparator = '|';

		/// <summary>
		/// The character used to decypher commands from other arguments.
		/// </summary>
		public const char ArgSpecifier  = '-';

		public Dictionary<string, CommandInfo> Callbacks;

		public CommandParser()
		{
			Callbacks = new Dictionary<string, CommandInfo>();
		}

		public CommandInfo this[string name]
		{
			get
			{
				return Callbacks[name];
			}
			set
			{
				Add(name, value.NameOnly, value.Callback, value.Description);
			}
		}

		/// <summary>
		/// Splits name by NameSeparator and adds all of the results to Callbacks.
		/// </summary>
		/// <param name="name">Command name(s).</param>
		/// <param name="nameOnly">If set to <c>true</c>, the command only requires itself to be specified.</param>
		/// <param name="func">The callback to use on a match.</param>
		/// <param name="desc">The description of the command.</param>
		public void Add(string name, bool nameOnly, Action<string> func, string desc)
		{
			var names = name.Split(NameSeparator);

			foreach(var n in names)
			{
				if(!Callbacks.ContainsKey(n))
					Callbacks[n] = new CommandInfo(nameOnly, func, desc, name);
			}
		}

		/// <summary>
		/// Parse the specified args for commands.
		/// </summary>
		/// <param name="args">Arguments to parse.</param>
		public void Parse(string[] args)
		{
			for(uint i = 0; i < args.Length; ++i)
			{
				var arg = args[i];

				if(arg[0] != ArgSpecifier)
				{
					Log.Warning("Skipping argument: " + arg);
					continue;
				}

				// Skip the arg specifier
				var name = arg.Substring(1);

				if(!Callbacks.ContainsKey(name))
				{
					Log.Warning("Unknown command: " + arg);
					continue;
				}

				var command = Callbacks[name];

				// Increase i if the command requires a value and the next iteration won't overflow the argument list
				if(!command.NameOnly && i + 1 < args.Length)
					++i;

				command.Callback.Invoke(args[i]);
			}
		}
	}

	public struct CommandInfo
	{
		public Action<string> Callback;
		public string FullName;
		public string Description;
		public bool NameOnly;

		public CommandInfo(bool nameOnly, Action<string> callback, string desc = "None", string name = "Unknown")
		{
			Callback    = callback;
			FullName    = name;
			Description = desc;
			NameOnly    = nameOnly;
		}
	}
}