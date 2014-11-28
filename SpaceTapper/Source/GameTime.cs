using System;

namespace SpaceTapper
{
	public sealed class GameTime
	{
		/// <summary>
		/// The current frame's delta time, in seconds.
		/// </summary>
		/// <value>The delta time.</value>
		public float DeltaTime { get; private set; }

		/// <summary>
		/// Returns the current frame rate.
		/// </summary>
		/// <value>The frame rate.</value>
		public float FrameRate
		{
			get
			{
				return 1f / DeltaTime;
			}
		}

		DateTime _lastDelta;

		public GameTime()
		{
			_lastDelta = DateTime.UtcNow;
		}

		/// <summary>
		/// Updates the current delta time.
		/// </summary>
		public void Update()
		{
			DeltaTime  = (float)(DateTime.UtcNow - _lastDelta).TotalSeconds;
			_lastDelta = DateTime.UtcNow;
		}
	}
}