using System;
using System.Timers;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Scenes;
using SpaceTapper.Util;

namespace SpaceTapper.UI
{
	public sealed class TextField : UIElement
	{
		/// <summary>
		/// The cursor blink time in milliseconds.
		/// </summary>
		public const uint CursorBlinkTime = 650;

		/// <summary>
		/// The text object used by the field.
		/// </summary>
		public Text Text;

		/// <summary>
		/// The text cursor shape.
		/// </summary>
		public RectangleShape CursorShape;

		/// <summary>
		/// The background shape for the text field.
		/// </summary>
		public RectangleShape BackgroundShape;

		/// <summary>
		/// The color of the text.
		/// </summary>
		public Color TextColor = Color.White;

		/// <summary>
		/// The color of the background.
		/// </summary>
		public Color BackgroundColor = new Color(75, 75, 75, 155);

		/// <summary>
		/// The color to use for the cursor.
		/// </summary>
		public Color CursorColor = Color.White;

		/// <summary>
		/// The text color to use when the field is disabled.
		/// </summary>
		public Color DisabledTextColor = new Color(100, 100, 100, 255);

		/// <summary>
		/// The background color to use when the field is disabled.
		/// </summary>
		public Color DisabledBackgroundColor = new Color(50, 50, 50, 155);

		/// <summary>
		/// The default text to use when the text field is reset.
		/// </summary>
		public string DefaultText;

		/// <summary>
		/// The maximum length of the text. Set to -1 for unlimited length.
		/// </summary>
		public int MaxLength = -1;

		/// <summary>
		/// The amount of padding (in pixels) to use around the text.
		/// </summary>
		public float Padding = 5;

		/// <summary>
		/// The width to use for the background when no text is in the field.
		/// </summary>
		public float DefaultWidth = 75;

		Scene _scene;
		Timer _cursorBlinkTimer;
		bool _cursorEnabled;
		bool _processInput;

		#region Constructors / destructors

		public TextField(Scene scene, string defaultText, Font font, uint charSize = 24)
		{
			_scene = scene;

			Text = new Text(defaultText, font, charSize);
			Text.Origin = new Vector2f(-Padding, -Padding);

			DefaultText = defaultText;

			CursorShape = new RectangleShape();

			BackgroundShape = new RectangleShape();
			BackgroundShape.OutlineThickness = 1;
			BackgroundShape.OutlineColor = new Color(125, 125, 125, 155);

			_scene.Input.Keys.AddOrUpdate(Keyboard.Key.Return, OnEnterPressed);
			_scene.Input.KeyPressed   += OnKeyPressed;
			_scene.Input.TextEntered  += OnTextEntered;
			_scene.Input.MousePressed += OnMousePressed;

			_cursorBlinkTimer = new Timer(CursorBlinkTime);
			_cursorBlinkTimer.Elapsed += (sender, e) => OnCursorBlink();

			UpdateAll();
		}

		public TextField(Scene scene, Font font, uint charSize = 24)
			: this(scene, "", font, charSize)
		{
		}

		~TextField()
		{
			_scene.Input.Keys[Keyboard.Key.Return] -= OnEnterPressed;
			_scene.Input.KeyPressed   -= OnKeyPressed;
			_scene.Input.TextEntered  -= OnTextEntered;
			_scene.Input.MousePressed -= OnMousePressed;
		}

		#endregion
		#region Private methods

		void UpdateColors(bool enabled)
		{
			if(enabled)
			{
				Text.Color = TextColor;
				BackgroundShape.FillColor = BackgroundColor;

				_cursorEnabled = true;
				CursorShape.FillColor = CursorColor;

				_cursorBlinkTimer.Start();
			}
			else
			{
				Text.Color = DisabledTextColor;
				BackgroundShape.FillColor = DisabledBackgroundColor;

				_cursorBlinkTimer.Stop();
				_cursorEnabled = false;

				CursorShape.FillColor = Color.Transparent;
			}
		}

		protected override void OnEnableChanged(bool newValue)
		{
			UpdateColors(newValue);
		}

		void OnEnterPressed(bool pressed)
		{
			if(!pressed || !Enabled)
				return;

			Enabled = false;
		}

		void OnCursorBlink()
		{
			_cursorEnabled = !_cursorEnabled;
			CursorShape.FillColor = _cursorEnabled ? CursorColor : Color.Transparent;
		}

		void OnMousePressed(Mouse.Button button)
		{
			var rect = _scene.Game.Window.MouseRect();
			bool intersects = rect.Intersects(Transform.TransformRect(BackgroundShape.GetGlobalBounds()));

			// This kind of checking prevents the field from being enabled / disabled if clicking multiple times on it.
			if(intersects && !Enabled)
				Enabled = true;
			else if(!intersects && Enabled)
				Enabled = false;
		}

		void OnKeyPressed(Keyboard.Key key)
		{
			if(!Enabled)
				return;

			// Since key presses are processed first, we use _processInput to tell OnTextEntered if we should process the key string or not.
			// Checking this manually will prevent invalid characters from being added to the display string.
			switch(key)
			{
				case Keyboard.Key.Back:
				{
					if(Text.DisplayedString.Length > 0)
					{
						// Remove the last character from the display string.
						Text.DisplayedString = Text.DisplayedString.Remove(Text.DisplayedString.Length - 1);
						UpdateAll();
					}

					_processInput = false;
					break;
				}

				case Keyboard.Key.Return:
				case Keyboard.Key.Escape:
				case Keyboard.Key.Delete:
					_processInput = false;
					break;

				default:
					_processInput = true;
					break;
			}
		}

		void OnTextEntered(string key)
		{
			if(!Enabled || !_processInput || (MaxLength >= 0 && Text.DisplayedString.Length >= MaxLength))
				return;

			Text.DisplayedString += key;
			UpdateAll();
		}

		#endregion
		#region Public methods

		public override void Reset()
		{
			if(MaxLength < 0)
				Text.DisplayedString = DefaultText;
			else
				Text.DisplayedString = DefaultText.Substring(0, MaxLength);

			UpdateAll();
		}

		/// <summary>
		/// Updates the cursor and background.
		/// </summary>
		public void UpdateAll()
		{
			UpdateBackground();
			ResetCursor();
			UpdateColors(Enabled);
		}

		/// <summary>
		/// Updates the background size.
		/// </summary>
		public void UpdateBackground()
		{
			var padAmt = Padding * 2;

			if(Text.DisplayedString.Length > 0)
			{
				var bounds = Text.GetLocalBounds();

				BackgroundShape.Position = bounds.Position();
				BackgroundShape.Size     = new Vector2f(bounds.Right() + padAmt, bounds.Height + padAmt);
			}
			else
			{
				BackgroundShape.Size = new Vector2f(DefaultWidth, Text.CharacterSize + Padding);
			}
		}

		/// <summary>
		/// Sets the cursor position to the end of the text and updates the cursor's size.
		/// </summary>
		public void UpdateCursorPosition()
		{
			var gBounds = Text.GetGlobalBounds();

			if(Text.DisplayedString.Length > 0)
			{
				CursorShape.Position = new Vector2f(gBounds.Right() + 2, gBounds.Top);
				CursorShape.Size = new Vector2f(1, Text.GetLocalBounds().Height);
			}
			else
			{
				CursorShape.Position = BackgroundShape.GetGlobalBounds().Position() + new Vector2f(Padding, Padding);
				CursorShape.Size     = new Vector2f(1, BackgroundShape.Size.Y - (Padding * 2));
			}
		}

		/// <summary>
		/// Updates the cursor's position and resets the blink timer.
		/// </summary>
		public void ResetCursor()
		{
			UpdateCursorPosition();

			CursorShape.FillColor = CursorColor;

			_cursorEnabled = true;
			_cursorBlinkTimer.Stop();
			_cursorBlinkTimer.Start();
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;

			target.Draw(BackgroundShape, states);
			target.Draw(Text, states);
			target.Draw(CursorShape, states);
		}

		#endregion
	}
}