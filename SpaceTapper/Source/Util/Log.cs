using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpaceTapper.Util
{
	public static class Log
	{
		/// <summary>
		/// If not empty, writes all messages to specified path.
		/// </summary>
		public static string LogFile;

		/// <summary>
		/// If true, all unhandled exceptions get a log entry with a guessed caller location.
		/// Note that the location may be incorrect at runtime, due to inlining by the JIT compiler.
		/// </summary>
		public static bool LogExceptions = true;

		static Log()
		{
			AppDomain.CurrentDomain.UnhandledException +=
				(sender, e) => OnException((Exception)e.ExceptionObject);
		}

		public static void Info(string message, [CallerFilePath] string methodPath = "",
			[CallerLineNumber] int methodLine = 0)
		{
			Write(methodPath, methodLine, "INFO", message);
		}

		public static void Warning(string message, [CallerFilePath] string methodPath = "",
			[CallerLineNumber] int methodLine = 0)
		{
			Write(methodPath, methodLine, "WARNING", message);
		}

		public static void Error(string message, [CallerFilePath] string methodPath = "",
			[CallerLineNumber] int methodLine = 0)
		{
			Write(methodPath, methodLine, "ERROR", message);
		}

		static void Write(string methodPath, int methodLine, string type, string message)
		{
			var fmtMessage = String.Format("[{0}] ({1}:{2}) {3}: {4}",
				                 DateTime.Now.ToString("hh:mm:ss tt"),
								 Path.GetFileName(methodPath),
								 methodLine,
				                 type,
								 message);

			Console.WriteLine(fmtMessage);

			if(!String.IsNullOrEmpty(LogFile))
				File.AppendAllText(LogFile, fmtMessage + '\n');
		}

		static void Write(int stackIndex, string type, string message)
		{
			// stackIndex is increased by 1 to escape this method
			var stackFrame = new StackFrame(stackIndex + 1);
			var method     = stackFrame.GetMethod();

			Write(method == null ? "Unknown" : method.GetFullName(), 0, type, message);
		}

		static void OnException(Exception exception)
		{
			if(!LogExceptions)
				return;

			Write(exception.TargetSite.GetFullName(), 0, "ERROR",
				exception.Message + "\n" + exception.StackTrace + "\n");
		}
	}
}