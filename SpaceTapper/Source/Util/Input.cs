using System;
using System.Collections.Generic;
using SFML.Window;

namespace SpaceTapper.Util
{
	/// <summary>
	/// Simple class that provides delegates for input.
	/// </summary>
	public static class Input
	{
		public static InputDict Keys;
		public delegate void KeyDel(bool pressed);

		static Input()
		{
			Keys = new InputDict();
		}

		/// <summary>
		/// Checks for the key being in the Keys dictionary and invokes it.
		/// </summary>
		/// <param name="e">Event args.</param>
		/// <param name="pressed">Passed along to the delegate(s) executed.</param>
		public static void ProcessKey(KeyEventArgs e, bool pressed)
		{
			if(Keys.ContainsKey(e.Code))
				Keys[e.Code].Invoke(pressed);
		}

		/// <summary>
		/// Same functionality as a Dictionary, but with an overloaded index operator.
		/// </summary>
		public class InputDict : Dictionary<Keyboard.Key, KeyDel>
		{
			/// <summary>
			/// Does the same thing as a Dictionary, but prevents invalid key exceptions.
			/// NOTE: = will act as += when more than one delegate exists.
			/// </summary>
			/// <param name="key">Key.</param>
			public new KeyDel this[Keyboard.Key key]
			{
				get
				{
					return base[key];
				}
				set
				{
					if(base.ContainsKey(key))
						base[key] += value;
					else
						base[key] = value;
				}
			}
		}
	}
}