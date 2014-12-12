using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Entities;
using SpaceTapper.Scenes;
using SpaceTapper.Util;

namespace SpaceTapper.UI
{
	public sealed class Button : UIElement
	{
		/// <summary>
		/// The default text size.
		/// </summary>
		public const uint TextSize = 18;

		/// <summary>
		/// The text used by the button.
		/// </summary>
		/// <value>The text.</value>
		public Text Text { get; private set; }

		/// <summary>
		/// The scene reference used by the button for input.
		/// </summary>
		/// <value>The scene.</value>
		public Scene Scene { get; private set; }

		/// <summary>
		/// Indicates if the button is being highlighted.
		/// </summary>
		/// <value><c>true</c> if highlighted; otherwise, <c>false</c>.</value>
		public bool Highlighted { get; private set; }

		/// <summary>
		/// The color to use when the button is highlighted.
		/// </summary>
		public Color HighlightColor = Color.Red;

		/// <summary>
		/// The color to use when the button is highlighted while disabled.
		/// </summary>
		public Color DisabledHighlightColor = new Color(100, 0, 0, 255);

		/// <summary>
		/// The color to use when the button is idle while disabled.
		/// </summary>
		public Color DisabledIdleColor = new Color(115, 115, 115, 255);

		/// <summary>
		/// The color to use when the button is idle.
		/// </summary>
		public Color IdleColor = Color.White;

		/// <summary>
		/// Called when the button is pressed.
		/// </summary>
		public event Action Pressed = delegate {};

		public override FloatRect GlobalBounds
		{
			get
			{
				return Transform.TransformRect(Text.GetGlobalBounds());
			}
		}

		bool _intersects;
		bool _prevIntersects;

		#region Constructors / destructors

		public Button(Scene scene, Font font, Vector2f position, uint size, bool center, string text)
		{
			Scene = scene;

			Text = new Text(text, font, size);
			Text.Color = IdleColor;

			Position = position.Truncate();

			if(center)
				Text.Origin = (GlobalBounds.Size() / 2).Truncate();

			scene.Input.MouseButtons.AddOrUpdate(Mouse.Button.Left, OnMousePress);
			scene.Input.MouseMoved += OnMouseMoved;
		}

		public Button(Scene scene, Font font, Vector2f position, bool center, string text)
			: this(scene, font, position, TextSize, center, text)
		{
		}

		public Button(Scene scene, Font font, Vector2f position, uint size, string text)
			: this(scene, font, position, size, true, text)
		{
		}

		public Button(Scene scene, Font font, Vector2f position, string text)
			: this(scene, font, position, TextSize, true, text)
		{
		}

		~Button()
		{
			Scene.Input.MouseButtons[Mouse.Button.Left] -= OnMousePress;
			Scene.Input.MouseMoved -= OnMouseMoved;
		}

		#endregion
		#region Private methods

		protected override void OnEnableChanged(bool newValue)
		{
			Text.Color = newValue ? IdleColor : DisabledIdleColor;
		}

		void OnMousePress(bool pressed)
		{
			if(pressed || !Enabled)
				return;

			var pos  = Mouse.GetPosition(Scene.Game.Window);
			var rect = new FloatRect(pos.X, pos.Y, 1, 1);

			if(GlobalBounds.Intersects(rect))
				Pressed.Invoke();
		}

		// TODO: Native segfault from sfml-window on exit sometimes after adding this.
		void OnMouseMoved(MouseMoveEventArgs e)
		{
			if(!Enabled)
				return;

			var rect = new FloatRect(e.X, e.Y, 1, 1);

			// This kind of checking prevents resetting colors if they're set externally
			_prevIntersects = _intersects;
			_intersects = GlobalBounds.Intersects(rect);

			if(_intersects && !_prevIntersects)
				SetHighlighted(true);
			else if(!_intersects && _prevIntersects)
				SetHighlighted(false);
		}

		#endregion
		#region Public methods

		/// <summary>
		/// Calls the Pressed event if the button is enabled.
		/// </summary>
		public void Press()
		{
			if(!Enabled)
				return;

			Pressed.Invoke();
		}

		/// <summary>
		/// Makes the button highlighted if value is true, idle otherwise.
		/// </summary>
		/// <param name="value">If set to <c>true</c>, the button is highlighted.</param>
		public void SetHighlighted(bool value)
		{
			Highlighted = value;

			if(Highlighted)
				Text.Color = Enabled ? HighlightColor : DisabledHighlightColor;
			else
				Text.Color = Enabled ? IdleColor : DisabledIdleColor;
		}

		public override void Reset()
		{
			Text.Color = Enabled ? IdleColor : DisabledIdleColor;
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;
			target.Draw(Text, states);
		}

		#endregion
	}
}