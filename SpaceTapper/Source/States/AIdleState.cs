using System;
using SFML.Graphics;

namespace SpaceTapper
{
	public abstract class AIdleState : AState
	{
		public RectangleShape BackgroundShape { get; protected set; }

		public AIdleState(Game instance, bool active = false) : base(instance, active)
		{
			BackgroundShape = new RectangleShape(GInstance.Size);
			BackgroundShape.FillColor = new Color(10, 10, 10, 200);
		}

		public override void Draw(RenderWindow window)
		{
			window.Draw(BackgroundShape);
		}
	}
}