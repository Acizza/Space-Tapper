using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections.ObjectModel;
using SFML.Window;
using SpaceTapper.Util;

namespace SpaceTapper
{
	public static class Parameters
	{
		#region Internal code

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

		public static void Parse()
		{

		}

		public static void GetAll()
		{
			var types = from type in Assembly.GetExecutingAssembly().GetTypes()
						from method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
						let attr = method.GetCustomAttribute<ParameterAttribute>(true)
						where attr != null
						select new { Attribute = attr, Method = method };

			_all.Clear();

			foreach(var param in types)
				_all[param.Attribute] = (ParameterDel)Delegate.CreateDelegate(typeof(ParameterDel), param.Method);
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
			// TODO: Revamp CommandParser class and move all help things here too
			Program.PrintHelp(true);
		}

		#endregion
	}
}