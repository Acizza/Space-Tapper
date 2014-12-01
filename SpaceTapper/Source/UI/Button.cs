using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Entities;
using SpaceTapper.Scenes;
using SpaceTapper.Util;

namespace SpaceTapper.UI
{
	// TODO: Convert inheritence to Transformable, IResetable, Drawable
	public sealed class Button : Entity
	{
		public const uint TextSize = 18;

		public Color HoverColor    = Color.Red;
		public Color DisabledHoverColor = new Color(100, 0, 0, 255);
		public Color DisabledIdleColor  = new Color(115, 115, 115, 255);
		public Color IdleColor     = Color.White;

		/// <summary>
		/// Called when the button is pressed.
		/// </summary>
		public event Action Pressed = delegate {};

		public Text Text { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SpaceTapper.UI.Button"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
				Text.Color = _enabled ? IdleColor : DisabledIdleColor;
			}
		}

		bool _enabled = true;

		#region Constructors / destructors

		public Button(Scene scene, Font font, Vector2f position, uint size, bool center, string text) : base(scene)
		{
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
		#region Overrides

		public override FloatRect GlobalBounds
		{
			get
			{
				return Transform.TransformRect(Text.GetGlobalBounds());
			}
		}

		/// <summary>
		/// Gets or sets the size of the button's text object.
		/// </summary>
		/// <value>The character size.</value>
		public new uint Size
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

		public override void Reset()
		{
			Text.Color = Enabled ? IdleColor : DisabledIdleColor;
		}

		public override void Update(GameTime time)
		{
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;
			target.Draw(Text, states);
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

		#endregion
		#region Private methods

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
			Text.Color = GlobalBounds.Intersects(rect) ? HoverColor : IdleColor;
		}

		#endregion
	}
}