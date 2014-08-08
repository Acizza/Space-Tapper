using System;
using SFML.Graphics;

namespace SpaceTapper
{
	public abstract class AState
	{
		public Game GInstance;

		public bool Updating;
		public bool Drawing;

		public bool Active
		{
			get
			{
				return Updating && Drawing;
			}

			set
			{
				Updating = value;
				Drawing  = value;
			}
		}

		public AState(Game instance, bool active = true)
		{
			GInstance = instance;
			Active = active;
		}

		public abstract void Update(TimeSpan dt);
		public abstract void Render(RenderWindow window);
	}
}