using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SFML.Window;
using SFML.Graphics;

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

		public static Vector2f Size(this FloatRect rect)
		{
			return new Vector2f(rect.Width, rect.Height);
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