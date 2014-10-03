using System;
using System.Runtime.InteropServices;

namespace SpaceTapper
{
	public class LinuxInit
	{
		[DllImport("X11")]
		public static extern int XInitThreads();
	}
}