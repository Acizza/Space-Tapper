using System;
using SFML.Window;
using SFML.Graphics;
using System.Collections.Generic;

namespace SpaceTapper
{
	public abstract class AUIElement : AEntity
	{
		public List<Drawable> Drawables { get; protected set; }

		public AUIElement(Game instance) : base(instance)
		{
			Drawables = new List<Drawable>();
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= this.Transform;

			foreach(var drawable in Drawables)
				target.Draw(drawable, states);
		}
	}
}