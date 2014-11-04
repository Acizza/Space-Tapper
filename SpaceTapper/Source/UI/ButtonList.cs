using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SFML.Window;
using SpaceTapper.States;
using SpaceTapper.UI;

namespace SpaceTapper
{
	public class ButtonList : List<Button>
	{
		/// <summary>
		/// The State the ButtonList is receiving input from.
		/// </summary>
		/// <value>The state.</value>
		public State State { get; private set; }

		/// <summary>
		/// The previous button index that was selected.
		/// </summary>
		/// <value>The previous index.</value>
		public int PrevSelected { get; private set; }

		/// <summary>
		/// The current button index that is selected. When set, sets the currently selected button.
		/// Wraps if the index is out of bounds.
		/// </summary>
		/// <value>The button index.</value>
		public int Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				if(value < 0)
					value = Count - 1;
				else if(value >= Count)
					value = 0;

				PrevSelected = _selected;
				_selected    = value;

				var prev     = base[PrevSelected];
				var selected = base[value];

				prev.Text.Color     = prev.ClearColor;
				selected.Text.Color = selected.SelectColor;
			}
		}

		int _selected;

		public ButtonList(State state)
		{
			State = state;

			State.Input.Keys[Keyboard.Key.Up]   = OnUpKey;
			State.Input.Keys[Keyboard.Key.W]    = OnUpKey;
			State.Input.Keys[Keyboard.Key.Down] = OnDownKey;
			State.Input.Keys[Keyboard.Key.S]    = OnDownKey;

			State.Input.Keys[Keyboard.Key.Return] = OnEnterKey;
		}

		public ButtonList(State state, IEnumerable<Button> collection) : this(state)
		{
			AddRange(collection);
		}

		public ButtonList(State state, int capacity) : this(state)
		{
			Capacity = capacity;
		}

		~ButtonList()
		{
			State.Input.Keys[Keyboard.Key.Up]   -= OnUpKey;
			State.Input.Keys[Keyboard.Key.W]    -= OnUpKey;
			State.Input.Keys[Keyboard.Key.Down] -= OnDownKey;
			State.Input.Keys[Keyboard.Key.S]    -= OnDownKey;

			State.Input.Keys[Keyboard.Key.Return] -= OnEnterKey;
		}

		void OnUpKey(bool pressed)
		{
			if(!pressed)
				return;

			--Selected;
		}

		void OnDownKey(bool pressed)
		{
			if(!pressed)
				return;

			++Selected;
		}

		void OnEnterKey(bool pressed)
		{
			if(!pressed)
				return;

			base[_selected].Press();
		}
	}
}