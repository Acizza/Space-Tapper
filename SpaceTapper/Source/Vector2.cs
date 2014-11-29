using System;
using SFML.Window;

namespace SpaceTapper
{
	/// <summary>
	/// A simple 2 dimensional float vector. Stores previous values.
	/// </summary>
	public struct Vector2
	{
		public float X
		{
			get
			{
				return _x;
			}
			set
			{
				PrevX = _x;
				_x    = value;
			}
		}

		public float Y
		{
			get
			{
				return _y;
			}
			set
			{
				PrevY = _y;
				_y    = value;
			}
		}

		public float PrevX { get; private set; }
		public float PrevY { get; private set; }

		float _x;
		float _y;

		public Vector2(float x, float y) : this()
		{
			X = x;
			Y = y;
		}

		public static implicit operator Vector2(Vector2f vec)
		{
			return new Vector2(vec.X, vec.Y);
		}

		public static implicit operator Vector2f(Vector2 vec)
		{
			return new Vector2f(vec.X, vec.Y);
		}

		public static Vector2 operator +(Vector2 vec1, Vector2 vec2)
		{
			return new Vector2(vec1.X + vec2.X, vec1.Y + vec2.Y);
		}

		public static Vector2 operator +(Vector2 vec, float val)
		{
			return new Vector2(vec.X + val, vec.Y + val);
		}

		public static Vector2 operator *(Vector2 vec, float val)
		{
			return new Vector2(vec.X * val, vec.Y * val);
		}

		public static Vector2 operator /(Vector2 vec, int val)
		{
			return new Vector2(vec.X / val, vec.Y / val);
		}
	}
}