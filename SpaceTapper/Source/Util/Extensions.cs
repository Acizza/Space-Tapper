using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
	}
}