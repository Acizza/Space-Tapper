﻿using System;
using SFML.Window;
using System.Collections.Generic;
using SpaceTapper.Util;

namespace SpaceTapper
{
	public sealed class Input
	{
		public Game Game;

		public Dictionary<Keyboard.Key, Action<bool>> Keys;
		public Dictionary<Mouse.Button, Action<bool>> MouseButtons;

		public Dictionary<Keyboard.Key, bool> QueuedKeys;
		public Dictionary<Mouse.Button, bool> QueuedMouseButtons;

		/// <summary>
		/// Called when a key is due for processing. Return false to halt further progress.
		/// </summary>
		public event Func<Keyboard.Key, bool> OnKeyProcess;

		/// <summary>
		/// Called when a mouse button is due for processing. Return false to halt further progress.
		/// </summary>
		public event Func<Mouse.Button, bool> OnMouseProcess;

		public event Action<MouseMoveEventArgs> MouseMoved = delegate {};

		/// <summary>
		/// If enabled, all input will be added to a queue and flushed at the end of the frame.
		/// </summary>
		public bool QueueEnabled = true;

		#region Constructors / destructors

		public Input(Game game)
		{
			Game = game;

			Keys               = new Dictionary<Keyboard.Key, Action<bool>>();
			MouseButtons       = new Dictionary<Mouse.Button, Action<bool>>();
			QueuedKeys         = new Dictionary<Keyboard.Key, bool>();
			QueuedMouseButtons = new Dictionary<Mouse.Button, bool>();

			Game.Window.KeyPressed  += OnKeyPressed;
			Game.Window.KeyReleased += OnKeyReleased;
			Game.Window.MouseMoved  += OnMouseMoved;
			Game.Window.MouseButtonPressed  += OnMousePressed;
			Game.Window.MouseButtonReleased += OnMouseReleased;

			Game.EndFrame += Flush;
		}

		~Input()
		{
			Game.Window.KeyPressed  -= OnKeyPressed;
			Game.Window.KeyReleased -= OnKeyReleased;
			Game.Window.MouseMoved  -= OnMouseMoved;
			Game.Window.MouseButtonPressed  -= OnMousePressed;
			Game.Window.MouseButtonReleased -= OnMouseReleased;

			Game.EndFrame -= Flush;
		}

		#endregion
		#region Event handlers

		void OnKeyPressed(object sender, KeyEventArgs e)
		{
			ProcessKey(e.Code, true);
		}

		void OnKeyReleased(object sender, KeyEventArgs e)
		{
			ProcessKey(e.Code, false);
		}

		void OnMousePressed(object sender, MouseButtonEventArgs e)
		{
			ProcessMouse(e.Button, true);
		}

		void OnMouseReleased(object sender, MouseButtonEventArgs e)
		{
			ProcessMouse(e.Button, false);
		}

		void OnMouseMoved(object sender, MouseMoveEventArgs e)
		{
			if(OnMouseProcess != null && !OnMouseProcess.Invoke(Mouse.Button.Left))
				return;

			MouseMoved.Invoke(e);
		}

		#endregion
		#region Input processors

		/// <summary>
		/// If a key handler exists in Keys, the method will either put the key in QueuedKeys,
		/// or call it directly depending on QueueEnabled.
		/// </summary>
		/// <param name="key">Key to process.</param>
		/// <param name="pressed">The key state.</param>
		public void ProcessKey(Keyboard.Key key, bool pressed)
		{
			if(!Keys.ContainsKey(key))
				return;

			if(OnKeyProcess != null && !OnKeyProcess.Invoke(key))
				return;

			if(QueueEnabled)
			{
				if(QueuedKeys.ContainsKey(key))
					return;

				QueuedKeys.Add(key, pressed);
			}
			else
			{
				Keys[key].Invoke(pressed);
			}
		}

		/// <summary>
		/// If a button handler exists in MouseButtons, the method will either put the button in QueuedMouseButtons,
		/// or call it directly depending on QueueEnabled.
		/// </summary>
		/// <param name="button">Mouse button to process.</param>
		/// <param name="pressed">The mouse button state.</param>
		public void ProcessMouse(Mouse.Button button, bool pressed)
		{
			if(!MouseButtons.ContainsKey(button))
				return;

			if(OnMouseProcess != null && !OnMouseProcess.Invoke(button))
				return;

			if(QueueEnabled)
			{
				if(QueuedMouseButtons.ContainsKey(button))
					return;

				QueuedMouseButtons.Add(button, pressed);
			}
			else
			{
				MouseButtons[button].Invoke(pressed);
			}
		}

		#endregion
		#region Button press methods

		/// <summary>
		/// Returns true if the specified key is pressed and the OnKeyProcess event returns true.
		/// </summary>
		/// <returns><c>true</c> if the key is pressed; otherwise, <c>false</c>.</returns>
		/// <param name="key">The key to check.</param>
		public bool IsPressed(Keyboard.Key key)
		{
			if(OnKeyProcess != null && !OnKeyProcess.Invoke(key))
				return false;

			return Keyboard.IsKeyPressed(key);
		}

		/// <summary>
		/// Returns true if the specified mouse button is pressed and the OnMouseProcess event returns true.
		/// </summary>
		/// <returns><c>true</c> if the mouse button is pressed; otherwise, <c>false</c>.</returns>
		/// <param name="button">The mouse button to check.</param>
		public bool IsPressed(Mouse.Button button)
		{
			if(OnMouseProcess != null && !OnMouseProcess.Invoke(button))
				return false;

			return Mouse.IsButtonPressed(button);
		}

		#endregion

		public void Flush()
		{
			if(!QueueEnabled)
				return;

			foreach(var k in QueuedKeys)
				Keys[k.Key].Invoke(k.Value);

			QueuedKeys.Clear();

			foreach(var m in QueuedMouseButtons)
				MouseButtons[m.Key].Invoke(m.Value);

			QueuedMouseButtons.Clear();
		}
	}
}