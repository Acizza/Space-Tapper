using System;
using System.Runtime.InteropServices;
using SFML.Graphics;
using SpaceTapper.States;
using SpaceTapper.Util;

namespace SpaceTapper.States
{
	public abstract class ForegroundState : State
	{
		public RectangleShape Background;
		public Color BackgroundColor = new Color(10, 10, 10, 200);

		protected ForegroundState()
		{
			Init();
		}

		protected ForegroundState(uint drawOrder) : base(drawOrder)
		{
			Init();
		}

		protected ForegroundState(string name, bool active = false) : base(name, active)
		{
			Init();
		}

		protected ForegroundState(string name, uint drawOrder, bool active = false)
			: base(name, drawOrder, active)
		{
			Init();
		}

		void Init()
		{
			Background = new RectangleShape(Game.Size.ToFloat());
			Background.FillColor = BackgroundColor;
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			target.Draw(Background);
		}
	}
}