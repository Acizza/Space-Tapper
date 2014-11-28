using System;
using System.Collections.Generic;
using System.Linq;
using SpaceTapper.Entities;

namespace SpaceTapper.Physics
{
	public sealed class Collider
	{
		public enum Side
		{
			None,
			Left,
			Right,
			Top,
			Bottom
		}

		public static Side GetCollisionSide(Entity ent1, Entity ent2)
		{
			// TODO

			return Side.None;
		}

		public void Update(IEnumerable<Entity> entities)
		{
			foreach(var entity in entities)
			{
				foreach(var collider in entities.OfType<ICollidable>())
				{
					if(collider == entity)
						continue;

					if(collider.Collides(entity))
						collider.OnCollision(entity, GetCollisionSide((Entity)collider, entity));
				}
			}
		}
	}

	public interface ICollidable
	{
		bool Collides(Entity entity);
		void OnCollision(Entity entity, Collider.Side side);
	}
}