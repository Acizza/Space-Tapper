using System;
using System.Runtime.InteropServices;

namespace SpaceTapper
{
	// SFML carries a bug with threads that will cause Xlib to complain about XInitTheads() not being called.
	// Therefore, we have to wrap and call it manually.
	public static class LinuxUtil
	{
		[DllImportAttribute("X11")]
		public static extern int XInitThreads();
	}
}