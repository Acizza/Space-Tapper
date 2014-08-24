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

		bool mLastMouseState = true;
		bool mLastMouseStatus;

		public Button(AState state, string text, uint charSize = 20, bool center = true)
			: base(state)
		{
			Text = new Text(text, State.GInstance.Fonts["default"], charSize);

			if(center)
			{
				Text.Origin = new Vector2f((float)Math.Round(Text.GetLocalBounds().Width / 2),
					(float)Math.Round(Text.GetLocalBounds().Height / 2));
			}
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
			{
				Text.Color = SelectColor;

				if(Mouse.IsButtonPressed(Mouse.Button.Left) && !mLastMouseState)
					OnPressed.Invoke();
			}
			else if(!inBounds && mLastMouseStatus)
				Text.Color = ClearColor;

			mLastMouseState = Mouse.IsButtonPressed(Mouse.Button.Left);
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
	}
}