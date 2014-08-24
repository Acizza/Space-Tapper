using System;
using SFML.Window;
using SFML.Graphics;
using System.Collections.Generic;

namespace SpaceTapper
{
	/// <summary>
	/// Adds no extra functionality. Only for separation purposes.
	/// </summary>
	public abstract class AUIElement : AEntity
	{
		public AUIElement(AState state) : base(state)
		{
		}
	}
}