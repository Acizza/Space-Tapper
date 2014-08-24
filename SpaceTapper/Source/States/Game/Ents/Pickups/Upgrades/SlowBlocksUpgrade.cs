using System;

namespace SpaceTapper
{
	public class SlowBlocksUpgrade : AUpgrade
	{
		float mPrevBlockSpeed;

		public SlowBlocksUpgrade(AState state) : base(state)
		{
			Name = "Slower Blocks";
			ActiveTime = TimeSpan.FromSeconds(20);
		}

		public override void Invoke()
		{
			var spawner = State.GInstance.GetState<GameState>().BlockSpawner;
			var difficulty = spawner.Difficulty;

			mPrevBlockSpeed = difficulty.BlockSpeed;
			difficulty.BlockSpeed *= 0.75f;

			spawner.Difficulty = difficulty;
		}

		public override void Disable()
		{
			var spawner = State.GInstance.GetState<GameState>().BlockSpawner;
			var difficulty = spawner.Difficulty;

			difficulty.BlockSpeed = mPrevBlockSpeed;
			spawner.Difficulty = difficulty;
		}
	}
}