using System;
using System.IO;

namespace SpaceTapper.Util
{
	/// <summary>
	/// Provides very simple type-based logging.
	/// </summary>
	public static class Log
	{
		public static string File;
		public static bool LogExceptions = true;

		static Log()
		{
			AppDomain.CurrentDomain.UnhandledException += OnException;
		}

		public static void Info(params string[] message)
		{
			Write("INFO", message);
		}

		public static void Warning(params string[] message)
		{
			Write("WARNING", message);
		}

		public static void Error(params string[] message)
		{
			Write("ERROR", message);
		}

		static void Write(string type, params string[] messages)
		{
			string message = String.Format("[{0}] {1}: ", DateTime.Now.ToString("hh:mm:ss tt"), type);

			foreach(var msg in messages)
				message += msg;

			Console.WriteLine(message);

			if(!String.IsNullOrEmpty(File))
				System.IO.File.AppendAllText(File, message + '\n');
		}

		static void OnException(object sender, UnhandledExceptionEventArgs e)
		{
			if(!LogExceptions)
				return;

			Write("EXCEPTION", e.ExceptionObject.ToString());
		}
	}
}