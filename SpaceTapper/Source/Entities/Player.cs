using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Entities;
using SpaceTapper.Math;
using SpaceTapper.Physics;
using SpaceTapper.Scenes;
using SpaceTapper.Util;

namespace SpaceTapper
{
	public sealed class Player : Entity, ICollidable
	{
		public Keyboard.Key MoveLeft  = Keyboard.Key.A;
		public Keyboard.Key MoveRight = Keyboard.Key.D;
		public Keyboard.Key MoveUp    = Keyboard.Key.Space;

		public float MoveSpeed = 175;
		public float SlowingMultiplier = 2;

		public Vector2f MaxSpeed     = new Vector2f(350, 400);
		public Vector2f Acceleration = new Vector2f(600, 250);
		public Vector2 Velocity;

		public RectangleShape Shape { get; private set; }

		public override FloatRect GlobalBounds
		{
			get
			{
				return Transform.TransformRect(Shape.GetGlobalBounds());
			}
		}

		public override Vector2 Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size      = value;
				Shape.Size = value;
			}
		}

		public Color Color
		{
			get
			{
				return _color;
			}
			set
			{
				_color          = value;
				Shape.FillColor = value;
			}
		}

		Vector2f _size = new Vector2f(15, 15);
		Color _color   = Color.Green;

		float _slowingTick;

		public Player(Scene scene) : base(scene)
		{
			Shape = new RectangleShape(Size);
			Shape.Origin    = Size / 2;
			Shape.FillColor = Color;
		}

		public Player(Scene scene, Vector2f pos) : this(scene)
		{
			Shape.Position = pos;
		}

		public bool Collides(Entity entity)
		{
			return entity.GlobalBounds.Intersects(GlobalBounds);
		}

		public void OnCollision(Entity entity, Collider.Side side)
		{
			Console.WriteLine("Collision");
		}

		void UpdateVelocity(float delta)
		{
			if(!Scene.Input.IsPressed(MoveLeft) && !Scene.Input.IsPressed(MoveRight))
			{
				_slowingTick += SlowingMultiplier * delta;
				Velocity.X = MathUtil.Lerp(Velocity.X, 0, _slowingTick * delta);
			}
			else
			{
				_slowingTick = 0;
			}

			Velocity.Y += Acceleration.Y / 2 * delta;
		}

		void ProcessInput(float delta)
		{
			if(Scene.Input.IsPressed(MoveLeft))
				Velocity.X = MathUtil.Clamp(Velocity.X - Acceleration.X * delta, -MaxSpeed.X, MaxSpeed.X);

			if(Scene.Input.IsPressed(MoveRight))
				Velocity.X = MathUtil.Clamp(Velocity.X + Acceleration.X * delta, -MaxSpeed.X, MaxSpeed.X);

			if(Scene.Input.IsPressed(MoveUp))
				Velocity.Y = MathUtil.Clamp(Velocity.Y - Acceleration.Y * delta, -MaxSpeed.Y, MaxSpeed.Y);
		}

		public override void Update(GameTime time)
		{
			ProcessInput(time.DeltaTime);
			UpdateVelocity(time.DeltaTime);

			Position += Velocity * time.DeltaTime;
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;
			target.Draw(Shape, states);
		}
	}
}