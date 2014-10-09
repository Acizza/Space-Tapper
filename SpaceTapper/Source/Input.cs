using System;
using System.Collections.Generic;
using System.Diagnostics;
using SFML.Window;

namespace SpaceTapper
{
	/// <summary>
	/// Simple class that provides delegates for input.
	/// </summary>
	public class Input
	{
		/// <summary>
		/// Key callbacks.
		/// </summary>
		public InputDict<Keyboard.Key> Keys;

		/// <summary>
		/// Mouse button callbacks.
		/// </summary>
		public InputDict<Mouse.Button> Mouse;

		/// <summary>
		/// Polled keys to be flushed at the end of the frame.
		/// </summary>
		/// <value>The polled keys.</value>
		public Dictionary<Keyboard.Key, bool> PolledKeys { get; private set; }

		/// <summary>
		/// Polled mouse button input to be flushed at the end of the frame.
		/// </summary>
		/// <value>The polled buttons.</value>
		public Dictionary<Mouse.Button, bool> PolledButtons { get; private set; }

		public delegate void PressDel(bool pressed);
		public delegate bool ProcessKeyDel(Keyboard.Key key);
		public delegate bool ProcessMouseDel(Mouse.Button button);

		/// <summary>
		/// When true, the input system will push all input to a queue and flush it at the end of the current frame.
		/// When false, the input system executes all input as soon as it notices it.
		/// </summary>
		public bool UseQueue = true;

		/// <summary>
		/// Called on every key state change. Return false to halt further processing.
		/// </summary>
		public ProcessKeyDel OnKeyProcess;

		/// <summary>
		/// Called on every mouse state change. Return false to halt further processing.
		/// </summary>
		public ProcessMouseDel OnMouseProcess;

		/// <summary>
		/// Occurs when the mouse is moved.
		/// </summary>
		public event Action<MouseMoveEventArgs> MouseMoved = delegate {};

		public Input()
		{
			Keys  = new InputDict<Keyboard.Key>();
			Mouse = new InputDict<Mouse.Button>();

			PolledKeys = new Dictionary<Keyboard.Key, bool>();
			PolledButtons = new Dictionary<Mouse.Button, bool>();

			Game.Window.KeyPressed          += OnKeyPressed;
			Game.Window.KeyReleased         += OnKeyReleased;
			Game.Window.MouseButtonPressed  += OnMousePressed;
			Game.Window.MouseButtonReleased += OnMouseReleased;
			Game.Window.MouseMoved          += OnMouseMoved;
			Game.EndFrame 					+= OnEndFrame;
		}

		~Input()
		{
			Game.Window.KeyPressed          -= OnKeyPressed;
			Game.Window.KeyReleased         -= OnKeyReleased;
			Game.Window.MouseButtonPressed  -= OnMousePressed;
			Game.Window.MouseButtonReleased -= OnMouseReleased;
			Game.Window.MouseMoved          -= OnMouseMoved;
			Game.EndFrame 					-= OnEndFrame;
		}

		/// <summary>
		/// Internal call to call matching delegates from Keys.
		/// </summary>
		/// <param name="e">Event.</param>
		/// <param name="pressed">Passed to the matching delegate(s).</param>
		void ProcessKeys(KeyEventArgs e, bool pressed)
		{
			if(OnKeyProcess != null && !OnKeyProcess.Invoke(e.Code))
				return;

			if(!Keys.ContainsKey(e.Code))
				return;

			if(UseQueue)
			{
				if(PolledKeys.ContainsKey(e.Code))
					return;

				PolledKeys.Add(e.Code, pressed);
			}
			else
				Keys[e.Code].Invoke(pressed);
		}

		/// <summary>
		/// Internal call to call matching delegates from mouse buttons.
		/// </summary>
		/// <param name="e">Event.</param>
		/// <param name="pressed">Passed to the matching delegate(s).</param>
		void ProcessMouse(MouseButtonEventArgs e, bool pressed)
		{
			if(OnMouseProcess != null && !OnMouseProcess.Invoke(e.Button))
				return;

			if(!Mouse.ContainsKey(e.Button))
				return;

			if(UseQueue)
			{
				if(PolledButtons.ContainsKey(e.Button))
					return;

				PolledButtons.Add(e.Button, pressed);
			}
			else
				Mouse[e.Button].Invoke(pressed);
		}

		void OnEndFrame()
		{
			foreach(var k in PolledKeys)
				Keys[k.Key].Invoke(k.Value);

			foreach(var m in PolledButtons)
				Mouse[m.Key].Invoke(m.Value);

			PolledKeys.Clear();
			PolledButtons.Clear();
		}

		void OnKeyPressed(object sender, KeyEventArgs e)
		{
			ProcessKeys(e, true);
		}

		void OnKeyReleased(object sender, KeyEventArgs e)
		{
			ProcessKeys(e, false);
		}

		void OnMousePressed(object sender, MouseButtonEventArgs e)
		{
			ProcessMouse(e, true);
		}

		void OnMouseReleased(object sender, MouseButtonEventArgs e)
		{
			ProcessMouse(e, false);
		}

		void OnMouseMoved(object sender, MouseMoveEventArgs e)
		{
			MouseMoved.Invoke(e);
		}

		/// <summary>
		/// Same functionality as a Dictionary, but with an overloaded index operator.
		/// </summary>
		public class InputDict<T> : Dictionary<T, PressDel>
		{
			/// <summary>
			/// Does the same thing as a Dictionary, but prevents invalid key exceptions.
			/// NOTE: = will act as += when more than one delegate exists.
			/// </summary>
			/// <param name="index">Index.</param>
			public new PressDel this[T index]
			{
				get
				{
					return base[index];
				}
				set
				{
					if(base.ContainsKey(index))
						base[index] += value;
					else
						base[index] = value;
				}
			}
		}
	}
}