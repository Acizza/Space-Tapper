using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Window;
using SpaceTapper.Util;

namespace SpaceTapper.UI
{
	public sealed class ButtonList
	{
		public Input Input          { get; private set; }
		public List<Button> Buttons { get; private set; }

		public delegate bool ScrolledDel(int oldIdx, int newIdx, Button button, Keyboard.Key scrollKey);

		/// <summary>
		/// The minimum button index to use for scrolling.
		/// </summary>
		public int MinIndex;

		/// <summary>
		/// The maximum button index to use for scrolling.
		/// </summary>
		public int MaxIndex;

		/// <summary>
		/// Called when the button list is scrolled.
		/// </summary>
		public event ScrolledDel Scrolled;

		/// <summary>
		/// Gets or sets the button index. When set, calls SetIndex(value).
		/// </summary>
		/// <value>The button index.</value>
		public int Index
		{
			get
			{
				return _index;
			}
			set
			{
				SetIndex(value);
			}
		}

		int _index;

		public ButtonList(Input input)
		{
			Input   = input;
			Buttons = new List<Button>();

			input.Keys.AddOrUpdate(Keyboard.Key.W,    p => OnUpPressed(Keyboard.Key.W, p));
			input.Keys.AddOrUpdate(Keyboard.Key.Up,   p => OnUpPressed(Keyboard.Key.Up, p));
			input.Keys.AddOrUpdate(Keyboard.Key.S,    p => OnDownPressed(Keyboard.Key.S, p));
			input.Keys.AddOrUpdate(Keyboard.Key.Down, p => OnDownPressed(Keyboard.Key.Down, p));
			input.Keys.AddOrUpdate(Keyboard.Key.Return, OnEnterPressed);
		}

		public ButtonList(Input input, params Button[] buttons) : this(input)
		{
			Add(buttons);
		}

		#region Public methods

		/// <summary>
		/// Ditto for Buttons.AddRange(). Sets MaxIndex to the total number of buttons.
		/// </summary>
		/// <param name="buttons">Buttons.</param>
		public void Add(params Button[] buttons)
		{
			Buttons.AddRange(buttons);
			MaxIndex = Buttons.Count - 1;
		}

		/// <summary>
		/// Sets the button index. Wraps if out of bounds of MinIndex or MaxIndex.
		/// </summary>
		/// <param name="index">The button index.</param>
		public void SetIndex(int index)
		{
			if(Buttons.Count == 0)
				return;

			_SetIndex(GetClosestIndex(index));
		}

		/// <summary>
		/// Gets the closest index to the one specified. Wraps if out of bounds of MinIndex or MaxIndex.
		/// </summary>
		/// <returns>The closest index found.</returns>
		/// <param name="index">The index to get the closest of.</param>
		public int GetClosestIndex(int index)
		{
			if(index >= Buttons.Count || index > MaxIndex)
				return MinIndex;

			if(index < 0 || index < MinIndex)
				return MaxIndex;

			return index;
		}

		#endregion
		#region Private methods

		/// <summary>
		/// Sets the current button index with no bounds checking.
		/// </summary>
		/// <param name="index">Index.</param>
		void _SetIndex(int index)
		{
			var cButton = Buttons[index];
			var pButton = Buttons[_index];

			pButton.Text.Color = pButton.Enabled ? pButton.IdleColor : pButton.DisabledIdleColor;
			cButton.Text.Color = cButton.Enabled ? cButton.HoverColor : cButton.DisabledHoverColor;

			_index = index;
		}

		void OnUpPressed(Keyboard.Key key, bool pressed)
		{
			if(pressed)
				return;

			int newIdx  = GetClosestIndex(Index - 1);
			bool result = true;

			if(Scrolled != null)
				result = Scrolled.Invoke(Index, newIdx, Buttons[newIdx], key);

			if(result)
				_SetIndex(newIdx);
		}

		void OnDownPressed(Keyboard.Key key, bool pressed)
		{
			if(pressed)
				return;

			int newIdx  = GetClosestIndex(Index + 1);
			bool result = true;

			if(Scrolled != null)
				result = Scrolled.Invoke(Index, newIdx, Buttons[newIdx], key);

			if(result)
				_SetIndex(newIdx);
		}

		void OnEnterPressed(bool pressed)
		{
			if(pressed || Buttons.Count == 0)
				return;

			Buttons[Index].Press();
		}

		#endregion
	}
}