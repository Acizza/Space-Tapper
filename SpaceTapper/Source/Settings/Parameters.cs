using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SFML.Window;
using SpaceTapper.Util;

namespace SpaceTapper.Settings
{
	public static class Parameters
	{
		#region Internal code

		public const char NameSeparator = '|';
		public const char ArgSpecifier  = '-';

		public delegate void ParameterDel(ref GameSettings settings, string value);

		public static ReadOnlyDictionary<ParameterAttribute, ParameterDel> All
		{
			get
			{
				return new ReadOnlyDictionary<ParameterAttribute, ParameterDel>(_all);
			}
		}

		static Dictionary<ParameterAttribute, ParameterDel> _all;

		static Parameters()
		{
			_all = new Dictionary<ParameterAttribute, ParameterDel>();
			GetAll();
		}

		/// <summary>
		/// Parse the specified arguments for parameters and execute them.
		/// </summary>
		/// <param name="settings">The game settings to pass to parameters for possible modification.</param> 
		/// <param name="args">Arguments to parse.</param>
		public static void Parse(ref GameSettings settings, string[] args)
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

				if(!_all.Select(x => x.Key.Name).Contains(name))
				{
					Log.Warning("Unknown command: " + arg);
					continue;
				}

				var command = _all.First(x => x.Key.Name == name);

				// Increase i if the command requires a value and the next iteration won't overflow the argument list
				if(command.Key.ValueNeeded && i + 1 < args.Length)
					++i;

				command.Value.Invoke(ref settings, args[i]);
			}
		}

		/// <summary>
		/// Uses reflection to grab all public static methods that have the Parameter attribute.
		/// Adds everything found to <see cref="Parameters.All"/>.
		/// </summary>
		public static void GetAll()
		{
			var types = from type in Assembly.GetExecutingAssembly().GetTypes()
						from method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
						let attr = method.GetCustomAttribute<ParameterAttribute>(true)
						where attr != null
						select new { Attribute = attr, Method = method };

			_all.Clear();

			foreach(var param in types)
			{
				// Split the name by NameSeparator and add them all manually
				var names = param.Attribute.Name.Split(NameSeparator);

				foreach(var name in names)
				{
					_all.Add(new ParameterAttribute(
						name,
						param.Attribute.ValueNeeded,
						param.Attribute.Description,
						param.Attribute.Name),
						(ParameterDel)Delegate.CreateDelegate(typeof(ParameterDel), param.Method));
				}
			}
		}

		#endregion
		#region Parameters

		[Parameter("w|width", true, "Sets the width of the window.")]
		public static void SetWidth(ref GameSettings settings, string value)
		{
			settings.Mode.Width = uint.Parse(value);
		}

		[Parameter("h|height", true, "Sets the height of the window.")]
		public static void SetHeight(ref GameSettings settings, string value)
		{
			settings.Mode.Height = uint.Parse(value);
		}

		[Parameter("v|vsync", true, "A true or false value indicating if vertical sync should be enabled.")]
		public static void SetVsync(ref GameSettings settings, string value)
		{
			settings.Vsync = bool.Parse(value);
		}

		[Parameter("a|autosize", false,
			"Sizes the window to the primary monitor's resolution. Must be after and width / height commands.")]
		public static void SetAutosize(ref GameSettings settings, string value)
		{
			settings.Mode = VideoMode.DesktopMode;
		}

		[Parameter("f|fullscreen", true, "Sets the window to fullscreen mode.")]
		public static void SetFullscreen(ref GameSettings settings, string value)
		{
			settings.Style = Styles.Fullscreen;
		}

		[Parameter("minorVersion", true, "The minor OpenGL version to use.")]
		public static void SetMinorVersion(ref GameSettings settings, string value)
		{
			settings.CSettings.MinorVersion = uint.Parse(value);
		}

		[Parameter("majorVersion", true, "The major OpenGL version to use.")]
		public static void SetMajorVersion(ref GameSettings settings, string value)
		{
			settings.CSettings.MajorVersion = uint.Parse(value);
		}

		[Parameter("aa|antialiasing", true, "The anti-aliasing level to use for the window.")]
		public static void SetAntialiasingLevel(ref GameSettings settings, string value)
		{
			settings.CSettings.AntialiasingLevel = uint.Parse(value);
		}

		[Parameter("log", true, "The file to write all log information to.")]
		public static void SetLogFile(ref GameSettings settings, string value)
		{
			Log.LogFile = value;
		}

		[Parameter("help|commands", false, "Prints this.")]
		public static void PrintHelp(ref GameSettings settings, string value)
		{
			Console.WriteLine("Specify command values by placing a space after typing the command.");
			Console.WriteLine("Commands separated by \"{0}\" do the same thing.\n", NameSeparator);

			foreach(var param in _all.DistinctBy(x => x.Key.FullName))
			{
				Console.WriteLine("{0}{1}:\n\tDescription: {2}\n\tValue needed: {3}\n",
					ArgSpecifier,
					param.Key.FullName,
					param.Key.Description,
					param.Key.ValueNeeded ? "yes" : "no");
			}

			Environment.Exit(0);
		}

		#endregion
	}
}