using System;

namespace SpaceTapper
{
	public class VelocityBoostUpgrade : AUpgrade
	{
		float mPrevAccel;

		public VelocityBoostUpgrade(Game instance) : base(instance)
		{
			Name = "Velocity Boost";
			ActiveTime = TimeSpan.FromSeconds(20);
		}

		public override void Invoke()
		{
			mPrevAccel = Player.Acceleration.Y;
			Player.Acceleration.Y *= 1.75f;
		}

		public override void Disable()
		{
			Player.Acceleration.Y = mPrevAccel;
		}
	}
}

