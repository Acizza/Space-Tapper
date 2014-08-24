using System;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;
using System.Linq;

namespace SpaceTapper
{
	/// <summary>
	/// Transforms a list of buttons into a key-selectable list.
	/// </summary>
	public class ButtonList
	{
		public List<Button> Buttons;
		public AState State;

		public int MaxIndex;

		public int MinIndex
		{
			get
			{
				return mMinIndex;
			}
			set
			{
				mMinIndex = value;

				if(value > Index)
					mIndex = MathUtil.Clamp(value, MinIndex, MaxIndex - 1);
			}
		}

		public int Index
		{
			get
			{
				return mIndex;
			}
			set
			{
				Buttons[mIndex].Text.Color = Button.ClearColor;

				if(value >= Buttons.Count || value > MaxIndex)
					mIndex = MinIndex;
				else if(value < 0 || value < MinIndex)
					mIndex = MaxIndex;
				else
					mIndex = value;

				Buttons[mIndex].Text.Color = Button.SelectColor;
			}
		}

		public Button this[string btnName]
		{
			get
			{
				return Buttons[Buttons.FindIndex(x => x.Text.DisplayedString == btnName)];
			}
			set
			{
				Buttons[Buttons.FindIndex(x => x.Text.DisplayedString == btnName)] = value;
			}
		}

		/// <summary>
		/// Shortcut for Buttons[index].
		/// </summary>
		/// <param name="index">Index.</param>
		public Button this[int index]
		{
			get
			{
				return Buttons[index];
			}
			set
			{
				Buttons[index] = value;
			}
		}

		int mIndex;
		int mMinIndex;

		public ButtonList(AState state, params Button[] buttons)
		{
			Buttons = new List<Button>(buttons);
			State = state;

			MaxIndex = Buttons.Count - 1;

			State.OnKeyPressed += KeyPressedHandler;
		}

		public ButtonList(AState state)
		{
			Buttons = new List<Button>();
			State = state;

			State.OnKeyPressed += KeyPressedHandler;
		}

		~ButtonList()
		{
			State.OnKeyPressed -= KeyPressedHandler;
		}
			
		/// <summary>
		/// Shortcut to add a button to the button list and set the max index.
		/// </summary>
		/// <param name="button">Button.</param>
		public void Add(Button button)
		{
			Buttons.Add(button);
			MaxIndex = Buttons.Count - 1;
		}

		void KeyPressedHandler(KeyEventArgs e)
		{
			switch(e.Code)
			{
				case Keyboard.Key.Up:
				case Keyboard.Key.W:
					--Index;
					break;

				case Keyboard.Key.Down:
				case Keyboard.Key.S:
					++Index;
					break;

				case Keyboard.Key.Return:
					Buttons[Index].Invoke();
					break;
			}
		}
	}
}