using System;
using SFML.Graphics;
using SpaceTapper.States;
using SpaceTapper.Util;

namespace SpaceTapper
{
	public abstract class ForegroundState : State
	{
		public RectangleShape Background;
		public Color BackgroundColor = new Color(10, 10, 10, 200);

		public ForegroundState()
		{
			Init();
		}

		public ForegroundState(uint drawOrder) : base(drawOrder)
		{
			Init();
		}

		public ForegroundState(string name, bool active = false) : base(name, active)
		{
			Init();
		}

		public ForegroundState(string name, uint drawOrder, bool active = false)
			: base(name, drawOrder, active)
		{
			Init();
		}

		void Init()
		{
			Background = new RectangleShape(Game.Size.ToFloat());
			Background.FillColor = BackgroundColor;
		}

		public override void Draw(RenderTarget target)
		{
			target.Draw(Background);
		}
	}
}