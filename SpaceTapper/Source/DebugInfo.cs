using System;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using System.Timers;

namespace SpaceTapper
{
	public sealed class DebugInfo : Transformable, IUpdatable, Drawable
	{
		public const uint DefaultFrameSamples = 150;

		public Text FrameRateText { get; private set; }
		public Text FrameTimeText { get; private set; }
		public Text MemoryText    { get; private set; }

		/// <summary>
		/// Used for frame rate averaging.
		/// Returns the number of frame samples being used.
		/// When set, it creates a new array with a size of the value passed in.
		/// </summary>
		/// <value>The frame sample count.</value>
		public uint FrameSamples
		{
			get
			{
				return _frameSampleCount;
			}
			set
			{
				_frameSampleCount = value;
				_frameSamples     = new float[value];
			}
		}

		uint _frameSampleCount;
		uint _frameSampleIndex;
		float[] _frameSamples;

		Timer _infoUpdater;

		public DebugInfo(TimeSpan updateInterval, uint frameSamples, Font textFont, uint textSize = 16)
		{
			FrameSamples = frameSamples;

			FrameRateText = new Text("FPS: 0.00", textFont, textSize);
			FrameTimeText = new Text("Frame Time: 0.00", textFont, textSize);
			MemoryText    = new Text("Mem: 0.00 MB", textFont, textSize);

			FrameTimeText.Position = new Vector2f(0, FrameRateText.GetLocalBounds().Height + 5);
			MemoryText.Position    = FrameTimeText.Position + new Vector2f(0, FrameTimeText.GetLocalBounds().Height + 5);

			_infoUpdater = new Timer(updateInterval.TotalMilliseconds);
			_infoUpdater.Elapsed += (sender, e) => UpdateInfo();
			_infoUpdater.Start();
		}

		public DebugInfo(TimeSpan updateInterval, Font textFont, uint textSize = 16)
			: this(updateInterval, DefaultFrameSamples, textFont, textSize)
		{
		}

		/// <summary>
		/// Adds fps to the frame samples. Wraps if out of bounds.
		/// </summary>
		/// <param name="fps">Frames per second.</param>
		void CollectFrameSample(float fps)
		{
			_frameSamples[_frameSampleIndex] = fps;

			if(++_frameSampleIndex >= _frameSamples.Length)
				_frameSampleIndex = 0;
		}

		void UpdateInfo()
		{
			// Using .Aggregate because it's faster than .Sum
			var fpsAverage = _frameSamples.Aggregate((a, b) => a + b) / FrameSamples;

			FrameRateText.DisplayedString = String.Format("FPS: {0:0.00}", fpsAverage);
			FrameTimeText.DisplayedString = String.Format("Frame Time: {0:0.000000}", 1f / fpsAverage);

			MemoryText.DisplayedString = String.Format("Mem: {0:0.00} MB",
				GC.GetTotalMemory(false) / 1024f / 1024f);
		}

		public void Update(GameTime time)
		{
			CollectFrameSample(time.FrameRate);
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;

			target.Draw(FrameRateText, states);
			target.Draw(FrameTimeText, states);
			target.Draw(MemoryText, states);
		}
	}
}