using System;
using SFML.Window;

namespace SpaceTapper
{
	public struct GameSettings
	{
		public string Title;
		public VideoMode Mode;
		public bool Vsync;
		public bool Fullscreen;
		public bool KeyRepeat;
	}
}