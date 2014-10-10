using System;
using System.Collections.Generic;
using SFML.Graphics;
using SpaceTapper.States;

namespace SpaceTapper
{
	public abstract class Entity : Transformable, Drawable
	{
		public List<Entity> Children;
		public State State { get; protected set; }

		public Entity(State state)
		{
			Children = new List<Entity>();
			State    = state;
		}

		protected abstract void UpdateSelf(float dt);
		protected abstract void DrawSelf(RenderTarget target, RenderStates states);

		public void Update(float dt)
		{
			UpdateSelf(dt);

			foreach(var child in Children)
				child.Update(dt);
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= base.Transform;

			DrawSelf(target, states);

			foreach(var child in Children)
				child.Draw(target, states);
		}
	}
}