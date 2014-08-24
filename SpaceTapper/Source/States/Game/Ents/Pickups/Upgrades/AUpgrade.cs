using System;

namespace SpaceTapper
{
	public abstract class AUpgrade : AEntity
	{
		public string Name { get; protected set; }
		public TimeSpan ActiveTime { get; protected set; }

		public AUpgrade(AState state) : base(state)
		{
		}

		public AUpgrade(AState state, string name, bool active = true)
			: this(state)
		{
			Name = name;
		}

		public abstract void Invoke();
		public abstract void Disable();
	}
}