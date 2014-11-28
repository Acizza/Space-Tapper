using System;

namespace SpaceTapper.Math
{
	public static class MathUtil
	{
		public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
		{
			if(val.CompareTo(min) < 0) return min;
			if(val.CompareTo(max) > 0) return max;

			return val;
		}

		public static float Lerp(float a, float b, float val)
		{
			return (1 - val) * a + val * b;
		}
	}
}