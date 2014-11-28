using System;
using System.Runtime.InteropServices;

namespace SpaceTapper.Util
{
	public static class NativeMethods
	{
		[DllImport("X11")]
		public static extern int XInitThreads();
	}
}