using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class Button : AUIElement
	{
		public static Font Font;
		public Text Text;
		public delegate void PressedDlg();
		public event PressedDlg Pressed = delegate {};

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

		bool lastMouseState;

		static Button()
		{
			Font = new Font("data/fonts/DejaVuSans.ttf");
		}

		public Button(Game instance, Vector2f pos, string text, uint charSize = 20, bool center = true) : base(instance)
		{
			Text = new Text(text, Font, charSize);
			Text.Position = pos;

			if(center)
				Text.Origin = new Vector2f((float)Math.Round(Text.GetLocalBounds().Width / 2),
											(float)Math.Round(Text.GetLocalBounds().Height / 2));

			Drawables.Add(Text);
		}

		public override void Update(TimeSpan dt)
		{
			if(MouseInBounds())
			{
				Text.Color = Color.Red;

				if(Mouse.IsButtonPressed(Mouse.Button.Left) && !lastMouseState)
					Pressed.Invoke();
			}
			else
				Text.Color = Color.White;

			lastMouseState = Mouse.IsButtonPressed(Mouse.Button.Left);
		}

		public bool MouseInBounds()
		{
			var m = Mouse.GetPosition(GInstance.Window);
			return Text.GetGlobalBounds().Intersects(new FloatRect(m.X, m.Y, 1, 1));
		}
	}
}