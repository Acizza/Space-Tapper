using System;
using System.Diagnostics;

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

			mLastDeltaUpdate = DateTime.UtcNow;
			mNextFpsReset    = DateTime.UtcNow.AddSeconds(FpsResetTime);
		}

		public void Update()
		{
			DeltaTime = DateTime.UtcNow - mLastDeltaUpdate;
			mLastDeltaUpdate = DateTime.UtcNow;

			++mFps;

			if(DateTime.UtcNow >= mNextFpsReset)
			{
				FpsUpdate.Invoke(mFps);

				Fps = mFps;
				mFps = 0;

				mNextFpsReset = DateTime.UtcNow.AddSeconds(FpsResetTime);
			}
		}
	}
}