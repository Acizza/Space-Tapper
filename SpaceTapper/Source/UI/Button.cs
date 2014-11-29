using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Entities;
using SpaceTapper.Scenes;
using SpaceTapper.Util;

namespace SpaceTapper
{
	// TODO: Convert inheritence to Transformable, IResetable, Drawable
	public sealed class Button : Entity
	{
		public const uint TextSize = 18;

		public Color HoverColor = Color.Red;
		public Color IdleColor  = Color.White;

		public event Action<Vector2i> Pressed = delegate {};

		public Text Text { get; private set; }

		#region Constructors / destructors

		public Button(Scene scene, Font font, Vector2f position, uint size, bool center, string text) : base(scene)
		{
			Text = new Text(text, font, size);
			Text.Color = IdleColor;

			Position = position;

			if(center)
				Text.Origin = GlobalBounds.Size() / 2;

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
			Text.Color = IdleColor;
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
		#region Private methods

		void OnMousePress(bool pressed)
		{
			if(pressed)
				return;

			var pos  = Mouse.GetPosition(Scene.Game.Window);
			var rect = new FloatRect(pos.X, pos.Y, 1, 1);

			if(GlobalBounds.Intersects(rect))
				Pressed.Invoke(pos);
		}

		// TODO: Getting a native segfault from the GC on exit after adding this. May also be related to not cleaning up manually.
		void OnMouseMoved(MouseMoveEventArgs e)
		{
			var rect = new FloatRect(e.X, e.Y, 1, 1);
			Text.Color = GlobalBounds.Intersects(rect) ? HoverColor : IdleColor;
		}

		#endregion
	}
}