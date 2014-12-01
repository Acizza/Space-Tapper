using System;
using SFML.Window;

namespace SpaceTapper
{
	public struct GameSettings
	{
		public VideoMode Mode;
		public ContextSettings CSettings;
		public Styles Style;
		public bool Vsync;
		public bool KeyRepeat;
		public string Title;
		public string DefaultScene;
	}
}