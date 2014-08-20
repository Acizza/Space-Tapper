using System;

namespace SpaceTapper
{
	public abstract class AUpgrade : AEntity
	{
		public string Name { get; protected set; }
		public TimeSpan ActiveTime { get; protected set; }

		public AUpgrade(Game instance) : base(instance)
		{
		}

		public AUpgrade(Game instance, string name) : this(instance)
		{
			Name = name;
		}

		public abstract void Invoke();
		public abstract void Disable();
	}
}