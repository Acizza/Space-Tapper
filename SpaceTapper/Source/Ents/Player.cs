using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	// TODO: Refactor entire class
	public class Player : Transformable, Drawable
	{
		public RectangleShape Shape { get; private set; }
		public Vector2f Velocity;
		public bool Alive;

		public readonly Vector2f Size = new Vector2f(15, 15);
		public readonly Vector2f MaxSpeed = new Vector2f(300, 400);
		public readonly Vector2f Acceleration = new Vector2f(600, 250);

		public Player(Vector2f pos)
		{
			Shape = new RectangleShape(Size);
			Shape.FillColor = Color.Green;

			Alive = true;

			Position = pos;
			Origin = Size / 2;
		}

		public void Update(float dt)
		{
			UpdateVelocity(dt);

			Position += Velocity * dt;

			if(Position.Y - Origin.Y >= Game.Instance.Window.Size.Y)
			{
				// TODO: Replace with Alive = false and adjust other code accordingly.
				Game.Instance.EndGame();
			}
		}

		public void Draw(RenderTarget target, RenderStates states)
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

		private void UpdateVelocity(float dt)
		{
			Velocity.X = MathUtil.Lerp(0, Velocity.X, 4 * dt);
			Velocity.Y += Acceleration.Y / 2 * dt;

			CheckInput(dt);
		}

		private void CheckInput(float dt)
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