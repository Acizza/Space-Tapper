using System;

namespace SpaceTapper
{
	public class GameTime
	{
		public TimeSpan DeltaTime { get; private set; }
		public uint  Fps { get; private set; }
		public float FpsResetTime;

		public delegate void OnFpsUpdate(uint fps);
		public event OnFpsUpdate FpsUpdate = delegate {};

		uint fps;
		DateTime lastDeltaUpdate;
		DateTime nextFpsReset;

		public GameTime(float fpsResetTime = 0.5f)
		{
			FpsResetTime = fpsResetTime;

			lastDeltaUpdate = DateTime.Now;
			nextFpsReset    = DateTime.Now.AddSeconds(FpsResetTime);
		}

		public void Update()
		{
			DeltaTime = DateTime.Now - lastDeltaUpdate;
			lastDeltaUpdate = DateTime.Now;

			++fps;

			if(DateTime.Now >= nextFpsReset)
			{
				FpsUpdate.Invoke(fps);

				Fps = fps;
				fps = 0;

				nextFpsReset = DateTime.Now.AddSeconds(FpsResetTime);
			}
		}
	}
}

