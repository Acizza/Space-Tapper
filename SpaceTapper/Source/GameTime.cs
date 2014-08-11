using System;

namespace SpaceTapper
{
	public class GameTime
	{
		public TimeSpan DeltaTime { get; private set; }
		public uint  Fps { get; private set; }
		public float FpsResetTime;

		public event Action<uint> FpsUpdate = delegate {};

		uint mFps;
		DateTime mLastDeltaUpdate;
		DateTime mNextFpsReset;

		public GameTime(float fpsResetTime = 0.5f)
		{
			FpsResetTime = fpsResetTime;

			mLastDeltaUpdate = DateTime.Now;
			mNextFpsReset    = DateTime.Now.AddSeconds(FpsResetTime);
		}

		public void Update()
		{
			DeltaTime = DateTime.Now - mLastDeltaUpdate;
			mLastDeltaUpdate = DateTime.Now;

			++mFps;

			if(DateTime.Now >= mNextFpsReset)
			{
				FpsUpdate.Invoke(mFps);

				Fps = mFps;
				mFps = 0;

				mNextFpsReset = DateTime.Now.AddSeconds(FpsResetTime);
			}
		}
	}
}