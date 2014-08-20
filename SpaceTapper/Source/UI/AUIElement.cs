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

		public override void DrawSelf(RenderTarget target, RenderStates states)
		{
			foreach(var drawable in Drawables)
				target.Draw(drawable, states);
		}
	}
}