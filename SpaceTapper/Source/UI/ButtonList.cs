using System;
using System.Collections.Generic;
using SFML.Window;
using SpaceTapper.Util;

namespace SpaceTapper.UI
{
	public sealed class ButtonList
	{
		public Input Input          { get; private set; }
		public List<Button> Buttons { get; private set; }

		/// <summary>
		/// The minimum button index to use for scrolling.
		/// </summary>
		public int MinIndex;

		/// <summary>
		/// The maximum button index to use for scrolling.
		/// </summary>
		public int MaxIndex;

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

			input.Keys.AddOrUpdate(Keyboard.Key.W, OnUpPressed);
			input.Keys.AddOrUpdate(Keyboard.Key.Up, OnUpPressed);
			input.Keys.AddOrUpdate(Keyboard.Key.S, OnDownPressed);
			input.Keys.AddOrUpdate(Keyboard.Key.Down, OnDownPressed);
			input.Keys.AddOrUpdate(Keyboard.Key.Return, OnEnterPressed);
		}

		public ButtonList(Input input, params Button[] buttons) : this(input)
		{
			Add(buttons);
		}

		~ButtonList()
		{
			Input.Keys[Keyboard.Key.W]      -= OnUpPressed;
			Input.Keys[Keyboard.Key.Up]     -= OnUpPressed;
			Input.Keys[Keyboard.Key.S]      -= OnDownPressed;
			Input.Keys[Keyboard.Key.Down]   -= OnDownPressed;
			Input.Keys[Keyboard.Key.Return] -= OnEnterPressed;
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

			if(index >= Buttons.Count || index > MaxIndex)
				index = MinIndex;
			else if(index < 0 || index < MinIndex)
				index = MaxIndex;

			var cButton = Buttons[index];
			var pButton = Buttons[_index];

			pButton.Text.Color = pButton.Enabled ? pButton.IdleColor : pButton.DisabledIdleColor;
			cButton.Text.Color = cButton.Enabled ? cButton.HoverColor : cButton.DisabledHoverColor;

			_index = index;
		}

		#endregion
		#region Private methods

		void OnUpPressed(bool pressed)
		{
			if(pressed)
				return;

			--Index;
		}

		void OnDownPressed(bool pressed)
		{
			if(pressed)
				return;

			++Index;
		}

		void OnEnterPressed(bool pressed)
		{
			if(pressed)
				return;

			Buttons[Index].Press();
		}

		#endregion
	}
}