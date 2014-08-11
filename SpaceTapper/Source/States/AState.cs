using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public abstract class AState
	{
		public Game GInstance;

		public bool Updating;
		public bool Drawing;

		public bool Active
		{
			get
			{
				return Updating && Drawing;
			}

			set
			{
				Updating = value;
				Drawing  = value;
			}
		}

		public AState(Game instance, bool active = true)
		{
			GInstance = instance;
			Active = active;

			GInstance.Window.KeyPressed += _OnKeyPressed;
		}

		public abstract void Update(TimeSpan dt);
		public abstract void Draw(RenderWindow window);

		protected virtual void OnKeyPressed(KeyEventArgs e)
		{
		}

		void _OnKeyPressed(object sender, KeyEventArgs e)
		{
			if(!Updating)
				return;

			OnKeyPressed(e);
		}
	}
}