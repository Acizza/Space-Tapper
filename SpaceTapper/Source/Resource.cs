using System;
using System.Collections.Generic;
using SpaceTapper.Util;

namespace SpaceTapper
{
	/// <summary>
	/// Provides a simple Dictionary that serves for resource caching.
	/// </summary>
	public class Resource<T>
	{
		public Dictionary<string, T> Resources;

		public Resource()
		{
			Resources = new Dictionary<string, T>();
		}

		public T this[string name]
		{
			get
			{
				return Get(name);
			}
			set
			{
				Add(name, value);
			}
		}

		/// <summary>
		/// Shortcut for Resources.Add().
		/// </summary>
		/// <param name="resource">Resource.</param>
		public void Add(string name, T resource)
		{
			Resources[name] = resource;
		}

		/// <summary>
		/// Shortcut for Resources[name], but with added checking. If the resource isn't found, returns the default value of T.
		/// </summary>
		/// <param name="name">Resource name.</param>
		public T Get(string name)
		{
			if(!Resources.ContainsKey(name))
			{
				Log.Error("Unknown ", typeof(T).ToString(), " resource: ", name);
				return default(T);
			}

			return Resources[name];
		}

		/// <summary>
		/// Shortcut for Resources.Remove.
		/// </summary>
		/// <param name="name">Resource name.</param>
		public void Remove(string name)
		{
			Resources.Remove(name);
		}
	}
}