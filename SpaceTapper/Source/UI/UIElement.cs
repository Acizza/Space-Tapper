using System;
using SFML.Graphics;

namespace SpaceTapper.UI
{
	public abstract class UIElement : Transformable, IResetable, Drawable
	{
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
				OnEnableChanged(value);
			}
		}

		public virtual FloatRect GlobalBounds
		{
			get
			{
				return Transform.TransformRect(new FloatRect(0, 0, 0, 0));
			}
		}

		bool _enabled;

		protected UIElement(bool enabled = true)
		{
			_enabled = enabled;
		}

		protected virtual void OnEnableChanged(bool newValue)
		{
		}

		public abstract void Reset();
		public abstract void Draw(RenderTarget target, RenderStates states);
	}
}