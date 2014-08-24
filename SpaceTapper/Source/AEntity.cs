using System;

namespace SpaceTapper
{
	public abstract class AEntity : SceneNode
	{
		public AState State;

		public AEntity(AState state)
		{
			State = state;
		}
	}
}