using System;
using SFML.Graphics;
using SpaceTapper.States;

namespace SpaceTapper
{
	[StateAttr]
	public class MenuState : State
	{
		public MenuState()
		{
			base.Name = "Menu";
		}

		public override void Update(double dt)
		{
		}

		public override void Draw(RenderTarget target)
		{
		}
	}
}