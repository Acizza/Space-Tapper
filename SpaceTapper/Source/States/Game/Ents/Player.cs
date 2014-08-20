using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class Player : ARectEntity
	{
		public Vector2f Velocity;
		public bool AllowSlowing;

		public event Action OnCollision = delegate {};

		public static readonly Vector2f Size = new Vector2f(15, 15);
		public static readonly Vector2f MaxSpeed = new Vector2f(300, 400);
		public static Vector2f Acceleration = new Vector2f(600, 250);

		public Player(Game instance) : base(instance)
		{
			Shape = new RectangleShape(Size);
			Shape.FillColor = Color.Green;

			Reset();
		}

		public override void UpdateSelf(TimeSpan delta)
		{
			var dt = (float)delta.TotalSeconds;

			UpdateVelocity(dt);
			Position += Velocity * dt;

			if(Position.Y - Origin.Y >= GInstance.Size.Y)
				OnCollision.Invoke();
		}

		public override void DrawSelf(RenderTarget target, RenderStates states)
		{
			target.Draw(Shape, states);
		}

		public void Reset()
		{
			Velocity = new Vector2f();
			Position = GInstance.Size / 2;
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

			if(AllowSlowing)
			{
				if(Keyboard.IsKeyPressed(Keyboard.Key.S))
					Velocity.Y += Acceleration.Y * dt;
			}
		}
	}
}