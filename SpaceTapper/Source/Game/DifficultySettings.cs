using System;

namespace SpaceTapper
{
	public struct DifficultySettings
	{
		public float BlockSpeed;
		public float BlockSpacing;
		public int   BlockCount;

		public DifficultySettings(float blockSpeed, float blockSpacing, int blockCount)
		{
			BlockSpeed   = blockSpeed;
			BlockSpacing = blockSpacing;
			BlockCount   = blockCount;
		}
	}
}