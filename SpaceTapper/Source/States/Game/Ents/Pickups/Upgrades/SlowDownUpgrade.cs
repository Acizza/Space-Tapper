using System;
using System.Threading;

namespace SpaceTapper
{
	public class SlowDownUpgrade : AUpgrade
	{
		public SlowDownUpgrade(AState state) : base(state)
		{
			Name = "Slow Down";
			ActiveTime = TimeSpan.FromSeconds(15);
		}

		public override void Invoke()
		{
			State.GInstance.GetState<GameState>().Player.AllowSlowing = true;
		}

		public override void Disable()
		{
			State.GInstance.GetState<GameState>().Player.AllowSlowing = false;
		}
	}
}