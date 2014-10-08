using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper.Util
{
	public static class Extensions
	{
		/// <summary>
		/// Returns the Shape's global bounds, with an offset.
		/// </summary>
		/// <returns>The transformed FloatRect.</returns>
		/// <param name="shape">Shape.</param>
		/// <param name="pos">Position.</param>
		public static FloatRect GlobalBounds(this Shape shape, Vector2f pos)
		{
			var b = shape.GetGlobalBounds();
			return new FloatRect(pos.X + b.Left, pos.Y + b.Top, b.Width, b.Height);
		}

		/// <summary>
		/// Applies a position offset to a FloatRect.
		/// </summary>
		/// <param name="rect">The FloatRect to take positions from.</param>
		/// <param name="pos">Position offset.</param>
		public static FloatRect Transform(this FloatRect rect, Vector2f pos)
		{
			return new FloatRect(pos.X + rect.Left, pos.Y + rect.Top, rect.Width, rect.Height);
		}

		/// <summary>
		/// Returns a Vector2f with rounded components.
		/// </summary>
		/// <param name="vec">Vector to take data from.</param>
		public static Vector2f Round(this Vector2f vec)
		{
			return new Vector2f((int)vec.X, (int)vec.Y);
		}

		/// <summary>
		/// Returns a new Vector2f with vec's values.
		/// </summary>
		/// <returns>The Vector2f.</returns>
		/// <param name="vec">Vector2i.</param>
		public static Vector2f ToFloat(this Vector2i vec)
		{
			return new Vector2f(vec.X, vec.Y);
		}
	}
}