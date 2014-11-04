using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SpaceTapper.Util
{
	/// <summary>
	/// Provides very simple type-based logging.
	/// </summary>
	public static class Log
	{
		/// <summary>
		/// The file name to write all messages to. If empty / null, no writing will be attempted.
		/// </summary>
		public static string File;

		/// <summary>
		/// If true, all exceptions will get a log entry.
		/// </summary>
		public static bool LogExceptions = true;

		static Log()
		{
			AppDomain.CurrentDomain.UnhandledException += OnException;
		}

		#region Public methods

		/// <summary>
		/// Writes to the console and a file if Log.File is not empty.
		/// </summary>
		/// <param name="message">Messages. Joined by whitespace automatically.</param>
		public static void Info(params string[] message)
		{
			Write("INFO", 2, message);
		}

		/// <summary>
		/// Writes to the console and a file if Log.File is not empty.
		/// </summary>
		/// <param name="message">Messages. Joined by whitespace automatically.</param>
		public static void Warning(params string[] message)
		{
			Write("WARNING", 2, message);
		}

		/// <summary>
		/// Writes to the console and a file if Log.File is not empty.
		/// </summary>
		/// <param name="message">Messages. Joined by whitespace automatically.</param>
		public static void Error(params string[] message)
		{
			Write("ERROR", 2, message);
		}

		#endregion
		#region Private logging methods

		/// <summary>
		/// Writes to the console and a file if File is not empty.
		/// </summary>
		/// <param name="type">Message type.</param>
		/// <param name="stackIndex">How far in the stack to go back for the method name.</param>
		/// <param name="message">Messages. Joined by whitespace.</param>
		static void Write(string type, int stackIndex, params string[] messages)
		{
			Write(type, stackIndex, String.Join(" ", messages));
		}

		/// <summary>
		/// Writes to the console and a file if File is not empty.
		/// </summary>
		/// <param name="type">Message type.</param>
		/// <param name="stackIndex">How far in the stack to go back for the method name.</param>
		/// <param name="message">Message.</param>
		static void Write(string type, int stackIndex, string message)
		{
			// Increase stackIndex by 1 to escape this method.
			var frame      = new StackFrame(stackIndex + 1);
			var methodName = frame.GetMethod().GetFullName();

			message = String.Format("[{0}] ({1}) {2}: {3}",
				DateTime.Now.ToString("hh:mm:ss tt"),
				methodName,
				type,
				message);

			Console.WriteLine(message);

			if(!String.IsNullOrEmpty(File))
				System.IO.File.AppendAllText(File, message + '\n');
		}

		static void OnException(object sender, UnhandledExceptionEventArgs e)
		{
			if(!LogExceptions)
				return;

			var exception = e.ExceptionObject as Exception;

			Error(exception.Message);
		}

		#endregion
	}
}