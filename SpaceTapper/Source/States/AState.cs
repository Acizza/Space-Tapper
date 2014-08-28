using System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public abstract class AState
	{
		public Game GInstance;

		public bool Updating
		{
			get
			{
				return mUpdating;
			}
			set
			{
				mUpdating = value;
				OnStatusChanged.Invoke(value, Drawing);
			}
		}

		public bool Drawing
		{
			get
			{
				return mDrawing;
			}
			set
			{
				mDrawing = value;
				OnStatusChanged.Invoke(Updating, value);
			}
		}

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

				OnStatusChanged.Invoke(value, value);
			}
		}

		bool mUpdating;
		bool mDrawing;

		public AState(Game instance, bool active = true)
		{
			GInstance = instance;
			Active = active;

			GInstance.Window.KeyPressed += _OnKeyPressed;
			GInstance.Window.MouseButtonPressed += _OnMousePressed;
		}

		~AState()
		{
			GInstance.Window.KeyPressed -= _OnKeyPressed;
			GInstance.Window.MouseButtonPressed -= _OnMousePressed;
		}

		public delegate void OnKeyPressedDlg(KeyEventArgs e);
		public delegate void OnMousePressedDlg(MouseButtonEventArgs e);
		public delegate void OnStatusChangeDlg(bool updating, bool drawing);

		public event OnKeyPressedDlg OnKeyPressed = delegate {};
		public event OnMousePressedDlg OnMousePressed = delegate {};
		public event OnStatusChangeDlg OnStatusChanged = delegate {};

		public abstract void Update(TimeSpan dt);
		public abstract void Draw(RenderWindow window);

		void _OnKeyPressed(object sender, KeyEventArgs e)
		{
			if(!Updating)
				return;

			OnKeyPressed.Invoke(e);
		}

		void _OnMousePressed(object sender, MouseButtonEventArgs e)
		{
			if(!Updating)
				return;

			OnMousePressed.Invoke(e);
		}
	}
}