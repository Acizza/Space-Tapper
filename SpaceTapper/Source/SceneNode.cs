using System;
using SFML.Graphics;
using System.Collections.Generic;
using SFML.Window;

namespace SpaceTapper
{
	// Slightly modified implementation from https://github.com/LaurentGomila/SFML-Game-Development-Book/blob/master/09_Audio/Source/SceneNode.cpp
	public class SceneNode : Transformable, Drawable
	{
		public List<SceneNode> Children;
		public SceneNode Parent;

		public Transform WorldTransform
		{
			get
			{
				var trans = Transform.Identity;

				for(SceneNode node = this; node != null; node = node.Parent)
					trans = node.Transform * trans;

				return trans;
			}
		}

		public Vector2f WorldPosition
		{
			get
			{
				return WorldTransform * new Vector2f();
			}
		}

		public SceneNode()
		{
			Children = new List<SceneNode>();
		}

		public void AddChild(SceneNode child)
		{
			child.Parent = this;
			Children.Add(child);
		}

		public void Update(TimeSpan dt)
		{
			UpdateSelf(dt);

			foreach(var child in Children)
				child.Update(dt);
		}

		public virtual void UpdateSelf(TimeSpan dt)
		{
		}

		public virtual void DrawSelf(RenderTarget target, RenderStates states)
		{
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;

			DrawSelf(target, states);

			foreach(var child in Children)
				child.Draw(target, states);
		}
	}
}