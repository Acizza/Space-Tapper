using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public static class MathUtil
	{
		public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if(val.CompareTo(min) < 0) return min;
			if(val.CompareTo(max) > 0) return max;

			return val;
		}

		public static float Lerp(this float val, float a, float b)
		{
			return (1 - val) * a + val * b;
		}
	}

	public static class SFMLUtil
	{
		public static Vector2f Size(this Text text)
		{
			var b = text.GetLocalBounds();
			return new Vector2f(b.Width, b.Height);
		}
	}
}