using System;
using System.Collections.Generic;

namespace SpaceTapper
{
	public class Resource<T>
	{
		public Dictionary<string, T> Resources { get; private set; }

		public Resource()
		{
			Resources = new Dictionary<string, T>();
		}

		public void Add(string name, T res)
		{
			Resources.Add(name, res);
		}

		public T Get(string name)
		{
			if(!Resources.ContainsKey(name))
				throw new Exception("Unknown " + typeof(T).ToString() + " resource: " + name);

			return Resources[name];
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
	}
}