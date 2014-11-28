using System;
using SFML.Graphics;
using SpaceTapper.Math;
using SpaceTapper.Scenes;

namespace SpaceTapper.Entities
{
	public abstract class Entity : Transformable, IUpdatable, Drawable
	{
		public Scene Scene { get; protected set; }

		public new Vector2 Position
		{
			get
			{
				return _position;
			}
			set
			{
				base.Position = value;
				_position     = value;
			}
		}

		public virtual Vector2 Size
		{
			get
			{
				return new Vector2(1, 1);
			}
			set
			{
			}
		}

		Vector2 _position;

		protected Entity(Scene scene)
		{
			Scene = scene;
		}

		public abstract FloatRect GlobalBounds { get; }

		public abstract void Update(GameTime time);
		public abstract void Draw(RenderTarget target, RenderStates states);
	}
}