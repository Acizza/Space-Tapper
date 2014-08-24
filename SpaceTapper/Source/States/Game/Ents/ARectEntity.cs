using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	/// <summary>
	/// Meant for simple entities that only require one RectangleShape.
	/// </summary>
	public class ARectEntity : AEntity
	{
		public RectangleShape Shape;

		/// <summary>
		/// Shortcut for Shape.GetGlobalBounds() with transformations applied.
		/// </summary>
		/// <value>The global bounds.</value>
		public FloatRect GlobalBounds
		{
			get
			{
				var b = Shape.GetGlobalBounds();
				return new FloatRect(WorldPosition.X, WorldPosition.Y, b.Width, b.Height);
			}
		}

		/// <summary>
		/// Shortcut for Shape.FillColor.
		/// </summary>
		/// <value>The color.</value>
		public Color FillColor
		{
			get
			{
				return Shape.FillColor;
			}
			set
			{
				Shape.FillColor = value;
			}
		}

		public ARectEntity(AState state) : base(state)
		{
			Shape = new RectangleShape();
		}

		public ARectEntity(AState state, Vector2f size)
			: this(state)
		{
			Shape.Size = size;
		}

		public override void DrawSelf(RenderTarget target, RenderStates states)
		{
			target.Draw(Shape, states);
		}
	}
}