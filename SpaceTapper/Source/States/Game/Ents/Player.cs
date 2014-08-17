using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class Player : AEntity
	{
		public RectangleShape Shape;
		public Vector2f Velocity;

		public event Action OnCollision = delegate {};

		public readonly Vector2f Size = new Vector2f(15, 15);
		public readonly Vector2f MaxSpeed = new Vector2f(300, 400);
		public readonly Vector2f Acceleration = new Vector2f(600, 250);

		/// <summary>
		/// Shortcut for Shape.GetGlobalBounds()
		/// </summary>
		/// <value>The global bounds.</value>
		public FloatRect GlobalBounds
		{
			get
			{
				return Shape.GetGlobalBounds();
			}
		}

		public Player(Game instance, Vector2f pos) : base(instance)
		{
			Shape = new RectangleShape(Size);
			Shape.FillColor = Color.Green;

			Shape.Origin = Size / 2;
			Reset();
		}

		public override void Update(TimeSpan delta)
		{
			var dt = (float)delta.TotalSeconds;

			UpdateVelocity(dt);
			Shape.Position += Velocity * dt;

			if(Shape.Position.Y - Shape.Origin.Y >= GInstance.Size.Y)
				OnCollision.Invoke();
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;
			target.Draw(Shape, states);
		}

		public void Reset()
		{
			Velocity = new Vector2f();
			Shape.Position = GInstance.Size / 2;
		}

		void UpdateVelocity(float dt)
		{
			Velocity.X = MathUtil.Lerp(0, Velocity.X, 4 * dt);
			Velocity.Y += Acceleration.Y / 2 * dt;

			CheckInput(dt);
		}

		void CheckInput(float dt)
		{
			if(Keyboard.IsKeyPressed(Keyboard.Key.A))
				Velocity.X = (Velocity.X - Acceleration.X * dt).Clamp(-MaxSpeed.X, MaxSpeed.X);

			if(Keyboard.IsKeyPressed(Keyboard.Key.D))
				Velocity.X = (Velocity.X + Acceleration.X * dt).Clamp(-MaxSpeed.X, MaxSpeed.X);

			if(Keyboard.IsKeyPressed(Keyboard.Key.Space))
				Velocity.Y -= Acceleration.Y * dt;
		}
	}
}