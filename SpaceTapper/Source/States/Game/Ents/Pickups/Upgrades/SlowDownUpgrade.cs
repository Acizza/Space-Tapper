using System;
using System.Threading;

namespace SpaceTapper
{
	public class SlowDownUpgrade : AUpgrade
	{
		public SlowDownUpgrade(Game instance) : base(instance)
		{
			Name = "Slow Down";
			ActiveTime = TimeSpan.FromSeconds(15);
		}

		public override void Invoke()
		{
			GInstance.GetState<GameState>().Player.AllowSlowing = true;
		}

		public override void Disable()
		{
			GInstance.GetState<GameState>().Player.AllowSlowing = false;
		}
	}
}