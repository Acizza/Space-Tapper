using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	// TODO: Refactor entire class
	public class Player : AEntity
	{
		public RectangleShape Shape;
		public Vector2f Velocity;
		public bool Alive;

		public event Action OnCollision = delegate {};

		public readonly Vector2f Size = new Vector2f(15, 15);
		public readonly Vector2f MaxSpeed = new Vector2f(300, 400);
		public readonly Vector2f Acceleration = new Vector2f(600, 250);

		public Player(Game instance, Vector2f pos) : base(instance)
		{
			Shape = new RectangleShape(Size);
			Shape.FillColor = Color.Green;

			Alive = true;

			Position = pos;
			Origin = Size / 2;
		}

		public override void Update(TimeSpan delta)
		{
			var dt = (float)delta.TotalSeconds;

			UpdateVelocity(dt);
			Position += Velocity * dt;

			if(Position.Y - Origin.Y >= GInstance.Size.Y)
				OnCollision.Invoke();
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;
			target.Draw(Shape, states);
		}

		public FloatRect GetGlobalBounds()
		{
			var bounds  = Shape.GetGlobalBounds();
			bounds.Left = Position.X - Origin.X;
			bounds.Top  = Position.Y - Origin.Y;

			return bounds;
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