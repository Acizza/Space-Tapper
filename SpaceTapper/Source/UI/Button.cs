using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class Button : AUIElement
	{
		public Text Text;
		public event Action OnPressed = delegate {};

		public static readonly Color ClearColor = new Color(255, 255, 255);
		public static readonly Color SelectColor = new Color(255, 0, 0);

		public uint CharacterSize
		{
			get
			{
				return Text.CharacterSize;
			}
			set
			{
				Text.CharacterSize = value;
			}
		}

		public FloatRect LocalBounds
		{
			get
			{
				return Text.GetLocalBounds();
			}
		}

		public new Vector2f Position
		{
			get
			{
				return Text.Position;
			}
			set
			{
				Text.Position = value;
			}
		}

		bool mLastMouseStatus;

		public Button(AState state, string text, uint charSize = 20, bool center = true) : base(state)
		{
			Text = new Text(text, State.GInstance.Fonts["default"], charSize);

			if(center)
				Text.Origin = new Vector2f((int)LocalBounds.Width / 2, (int)LocalBounds.Height / 2);

			State.OnMousePressed += HandleOnMousePressed;
		}

		public Button(AState state, Vector2f pos, string text, uint charSize = 20, bool center = true)
			: this(state, text, charSize, center)
		{
			Text.Position = pos;
		}

		public void Invoke()
		{
			OnPressed.Invoke();
		}

		public override void UpdateSelf(TimeSpan dt)
		{
			var inBounds = MouseInBounds();

			if(inBounds)
				Text.Color = SelectColor;
			else if(!inBounds && mLastMouseStatus) // Only set to the clear color if the mouse just left the button.
				Text.Color = ClearColor;

			mLastMouseStatus = MouseInBounds();
		}

		public override void DrawSelf(RenderTarget target, RenderStates states)
		{
			target.Draw(Text, states);
		}

		public bool MouseInBounds()
		{
			var m = Mouse.GetPosition(State.GInstance.Window);
			return Text.GetGlobalBounds().Intersects(new FloatRect(m.X, m.Y, 1, 1));
		}

		void HandleOnMousePressed(MouseButtonEventArgs e)
		{
			if(!MouseInBounds())
				return;

			OnPressed.Invoke();
		}
	}
}