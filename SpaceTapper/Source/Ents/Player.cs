using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.States;
using SpaceTapper.Util;

namespace SpaceTapper.Ents
{
	public class Player : Entity
	{
		/// <summary>
		/// The multiplier used when slowing down the player when no horizontal keys are pressed.
		/// </summary>
		public const float SlowingMultiplier  = 0.0075f;
		public static readonly Vector2f Size  = new Vector2f(15, 15);
		public static readonly Color    Color = Color.Green;

		public RectangleShape Shape { get; private set; }

		/// <summary>
		/// The key used to move left.
		/// </summary>
		public Keyboard.Key MoveLeft  = Keyboard.Key.A;

		/// <summary>
		/// The key used to move right.
		/// </summary>
		public Keyboard.Key MoveRight = Keyboard.Key.D;

		/// <summary>
		/// The key used to move upwards.
		/// </summary>
		public Keyboard.Key MoveUp    = Keyboard.Key.Space;

		/// <summary>
		/// The position the player will appear in when Reset() is called.
		/// </summary>
		public Vector2f InitialPosition;

		/// <summary>
		/// Player velocity.
		/// </summary>
		public Vector2f Velocity;

		/// <summary>
		/// How fast the player will move horizontally and vertically (also from gravity).
		/// </summary>
		public Vector2f Acceleration = new Vector2f(600, 250);

		/// <summary>
		/// All key-based movement is clamped to this.
		/// </summary>
		public Vector2f MaxSpeed     = new Vector2f(300, 400);

		/// <summary>
		/// Called when the player moves out of screen-space.
		/// </summary>
		public event Action HitWall = delegate {};

		bool _moveLeftPressed;
		bool _moveRightPressed;
		bool _moveUpPressed;

		float _slowingTick;

		public Player(State state, Vector2f pos) : base(state)
		{
			Shape = new RectangleShape(Size);
			Shape.FillColor = Color;

			Origin = Size / 2;

			InitialPosition = pos;
			Position        = pos;

			State.Input.Keys[MoveLeft]  = OnLeftPressed;
			State.Input.Keys[MoveRight] = OnRightPressed;
			State.Input.Keys[MoveUp]    = OnUpPressed;
		}

		~Player()
		{
			State.Input.Keys[MoveLeft]  -= OnLeftPressed;
			State.Input.Keys[MoveRight] -= OnRightPressed;
			State.Input.Keys[MoveUp]    -= OnUpPressed;
		}

		/// <summary>
		/// Resets the velocity and positions the player at InitialPosition.
		/// </summary>
		public void Reset()
		{
			Position = InitialPosition;
			Velocity = new Vector2f(0, 0);
		}

		void CheckWallCollision()
		{
			var x = Position.X - Origin.X;
			var y = Position.Y - Origin.Y;

			if((x < 0 || x > Game.Size.X) || (y < 0 || y > Game.Size.Y))
				HitWall.Invoke();
		}

		void UpdateVelocity(float dt)
		{
			if(!_moveLeftPressed && !_moveRightPressed)
			{
				_slowingTick += SlowingMultiplier * dt;
				Velocity.X = MathUtil.Lerp(Velocity.X, 0, _slowingTick);
			}
			else
				_slowingTick = 0;

			Velocity.Y += Acceleration.Y / 2 * dt;
			ProcessInput(dt);
		}

		void ProcessInput(float dt)
		{
			if(_moveLeftPressed)
				Velocity.X = MathUtil.Clamp(Velocity.X - Acceleration.X * dt, -MaxSpeed.X, MaxSpeed.X);

			if(_moveRightPressed)
				Velocity.X = MathUtil.Clamp(Velocity.X + Acceleration.X * dt, -MaxSpeed.X, MaxSpeed.X);

			if(_moveUpPressed)
				Velocity.Y = MathUtil.Clamp(Velocity.Y - Acceleration.Y * dt, -MaxSpeed.Y, MaxSpeed.Y);
		}

		void OnLeftPressed(bool pressed)
		{
			_moveLeftPressed = pressed;
		}

		void OnRightPressed(bool pressed)
		{
			_moveRightPressed = pressed;
		}

		void OnUpPressed(bool pressed)
		{
			_moveUpPressed = pressed;
		}

		protected override void UpdateSelf(float dt)
		{
			UpdateVelocity(dt);

			Position += Velocity * dt;

			CheckWallCollision();
		}

		protected override void DrawSelf(RenderTarget target, RenderStates states)
		{
			target.Draw(Shape, states);
		}
	}
}