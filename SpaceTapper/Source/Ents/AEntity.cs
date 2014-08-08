using System;
using SFML.Graphics;

namespace SpaceTapper
{
	public abstract class AEntity : Transformable, Drawable
	{
		public Game GInstance;

		public AEntity(Game instance)
		{
			GInstance = instance;
		}

		public abstract void Update(TimeSpan dt);
		public abstract void Draw(RenderTarget target, RenderStates states);
	}
}