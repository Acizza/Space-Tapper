using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper.Util
{
	public static class Extensions
	{
		/// <summary>
		/// Gets the full name (class + . + method name) of a method.
		/// </summary>
		/// <returns>The full name.</returns>
		/// <param name="method">Method.</param>
		public static string GetFullName(this MemberInfo method)
		{
			return method.ReflectedType.Name + "." + method.Name;
		}

		/// <summary>
		/// Adds an Action to a dictionary. If the key already exists, it appends the delegate to it.
		/// </summary>
		/// <param name="dict">Dictionary.</param>
		/// <param name="key">Dictionary key.</param>
		/// <param name="func">Delegate to add / append.</param>
		/// <typeparam name="K">The 1st dictionary type parameter.</typeparam>
		/// <typeparam name="V">The Action type parameters.</typeparam>
		public static void AddOrUpdate<K, V>(this Dictionary<K, Action<V>> dict, K key, Action<V> func)
		{
			if(dict.ContainsKey(key))
				dict[key] += func;
			else
				dict[key] = func;
		}

		public static IEnumerable<T> DistinctBy<T, K>(this IEnumerable<T> source, Func<T, K> selector)
		{
			return source.GroupBy(selector).Select(x => x.First());
		}

		/// <summary>
		/// Returns a truncated version of the specified vector.
		/// </summary>
		/// <param name="vec">The vector to truncate.</param>
		public static Vector2f Truncate(this Vector2f vec)
		{
			return new Vector2f((float)Math.Truncate(vec.X), (float)Math.Truncate(vec.Y));
		}

		/// <summary>
		/// Returns the size attributes of the specified FloatRect.
		/// </summary>
		/// <param name="rect">The FloatRect to use.</param>
		public static Vector2f Size(this FloatRect rect)
		{
			return new Vector2f(rect.Width, rect.Height);
		}

		/// <summary>
		/// Returns the position attributes of the specified FloatRect.
		/// </summary>
		/// <param name="rect">The FloatRect to use.</param>
		public static Vector2f Position(this FloatRect rect)
		{
			return new Vector2f(rect.Left, rect.Top);
		}

		/// <summary>
		/// Returns the X position + width of the specified FloatRect.
		/// </summary>
		/// <param name="rect">The FloatRect to use.</param>
		public static float Right(this FloatRect rect)
		{
			return rect.Left + rect.Width;
		}

		/// <summary>
		/// Returns a version of the specified string with the first letter capitalized.
		/// </summary>
		/// <returns>The string, with the first letter capitalized.</returns>
		/// <param name="str">The string to use.</param>
		public static string MakeFirstUpper(this string str)
		{
			return Char.ToUpper(str[0]) + str.Substring(1);
		}

		public static FloatRect MouseRect(this Window window)
		{
			var pos = Mouse.GetPosition(window);
			return new FloatRect(pos.X, pos.Y, 1, 1);
		}

		public static FloatRect MouseRect(this MouseMoveEventArgs e)
		{
			return new FloatRect(e.X, e.Y, 1, 1);
		}

		public static FloatRect MouseRect(this Vector2f pos)
		{
			return new FloatRect(pos.X, pos.Y, 1, 1);
		}

		public static Vector2f ToVector2f(this Vector2i vec)
		{
			return new Vector2f(vec.X, vec.Y);
		}

		public static Vector2f ToVector2f(this Vector2u vec)
		{
			return new Vector2f(vec.X, vec.Y);
		}
	}
}