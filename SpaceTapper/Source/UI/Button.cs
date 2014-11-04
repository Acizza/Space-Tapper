using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.States;
using SpaceTapper.Util;

namespace SpaceTapper.UI
{
	/// <summary>
	/// A simple text button. Note: calling update on this is not necessary.
	/// </summary>
	public class Button : Entity
	{
		/// <summary>
		/// The color to show when the mouse is not over the button.
		/// </summary>
		public Color ClearColor  = Color.White;

		/// <summary>
		/// The color to show when the mouse is over the button.
		/// </summary>
		public Color SelectColor = Color.Red;

		/// <summary>
		/// The text object.
		/// </summary>
		public Text Text;

		/// <summary>
		/// Gets a value indicating whether the mouse is in bounds.
		/// </summary>
		/// <value><c>true</c> if mouse is in bounds; otherwise, <c>false</c>.</value>
		public bool MouseInBounds { get; private set; }

		/// <summary>
		/// Occurs when the button is pressed.
		/// Note: If trying to call from an external class, use Press() instead.
		/// </summary>
		public event Action Pressed = delegate {};

		public Button(State state) : base(state)
		{
			State.Input.MButtons[Mouse.Button.Left] = OnMousePressed;
			State.Input.MouseMoved += OnMouseMoved;
		}

		~Button()
		{
			State.Input.MButtons[Mouse.Button.Left] -= OnMousePressed;
			State.Input.MouseMoved -= OnMouseMoved;
		}

		public Button(State state, string str, Font font, Vector2f pos, uint charSize = 21, bool center = true)
			: this(state)
		{
			Position = pos;

			Text = new Text(str, font, charSize);
			Text.Color = ClearColor;

			if(center)
				Center();
		}

		/// <summary>
		/// Invokes the Pressed event.
		/// </summary>
		public void Press()
		{
			Pressed.Invoke();
		}

		/// <summary>
		/// Centers the text origin.
		/// </summary>
		public void Center()
		{
			var b = Text.GetLocalBounds();
			Text.Origin = new Vector2f((int)b.Width / 2, (int)b.Height / 2);
		}

		protected override void UpdateSelf(float dt)
		{
		}

		protected override void DrawSelf(RenderTarget target, RenderStates states)
		{
			target.Draw(Text, states);
		}

		void OnMousePressed(bool pressed)
		{
			if(!MouseInBounds || pressed)
				return;

			Pressed.Invoke();
		}

		void OnMouseMoved(MouseMoveEventArgs e)
		{
			var rect = new FloatRect(e.X, e.Y, 1, 1);

			if(Text.GetGlobalBounds().Transform(Position).Intersects(rect))
			{
				MouseInBounds = true;
				Text.Color = SelectColor;
			}
			else
			{
				// TODO: If the button is in a ButtonList, the color resets when the mouse moves.
				MouseInBounds = false;
				Text.Color = ClearColor;
			}
		}
	}
}