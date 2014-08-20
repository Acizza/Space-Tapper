using System;

namespace SpaceTapper
{
	public class SlowBlocksUpgrade : AUpgrade
	{
		float mPrevBlockSpeed;

		public SlowBlocksUpgrade(Game instance) : base(instance)
		{
			Name = "Slower Blocks";
			ActiveTime = TimeSpan.FromSeconds(20);
		}

		public override void Invoke()
		{
			var spawner = GInstance.GetState<GameState>().BlockSpawner;
			var difficulty = spawner.Difficulty;

			mPrevBlockSpeed = difficulty.BlockSpeed;
			difficulty.BlockSpeed *= 0.75f;

			spawner.Difficulty = difficulty;
		}

		public override void Disable()
		{
			var spawner = GInstance.GetState<GameState>().BlockSpawner;
			var difficulty = spawner.Difficulty;

			difficulty.BlockSpeed = mPrevBlockSpeed;
			spawner.Difficulty = difficulty;
		}
	}
}