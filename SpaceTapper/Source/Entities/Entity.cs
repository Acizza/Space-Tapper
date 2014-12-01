using System;
using SFML.Graphics;
using SpaceTapper.Scenes;

namespace SpaceTapper.Entities
{
	public abstract class Entity : Transformable, IUpdatable, Drawable, IResetable
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

		public virtual FloatRect GlobalBounds
		{
			get
			{
				return new FloatRect(0, 0, 0, 0);
			}
		}

		public abstract void Reset();

		public abstract void Update(GameTime time);
		public abstract void Draw(RenderTarget target, RenderStates states);
	}
}