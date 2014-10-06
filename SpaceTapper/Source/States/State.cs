using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SFML.Graphics;

namespace SpaceTapper.States
{
	public abstract class State
	{
		public string Name;
		public Input  Input { get; private set; }

		public bool Updating
		{
			get
			{
				return mUpdating;
			}
			set
			{
				UpdateChanged(value);
				mUpdating = value;
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
				DrawChanged(value);
				mDrawing = value;
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
				StatusChanged(value);

				Updating = value;
				Drawing  = value;
			}
		}

		bool mUpdating;
		bool mDrawing;

		public State()
		{
			Input = new Input();

			// Only process input if the state is updating.
			Input.OnKeyProcess = k => Updating;
			Input.OnMouseProcess = m => Updating;
		}

		public State(string name, bool active = false) : this()
		{
			Name   = name;
			Active = active;
		}

		public abstract void Update(double dt);
		public abstract void Draw(RenderTarget target);

		/// <summary>
		/// Called when Active is set.
		/// </summary>
		/// <param name="activated">The new Active value.</param>
		public virtual void StatusChanged(bool activated)
		{
		}

		/// <summary>
		/// Called when Update is set.
		/// </summary>
		/// <param name="flag">The new Updating value.</param>
		public virtual void UpdateChanged(bool flag)
		{
		}

		/// <summary>
		/// Called when Drawing is set.
		/// </summary>
		/// <param name="flag">The new Drawing value.</param>
		public virtual void DrawChanged(bool flag)
		{
		}

		/// <summary>
		/// Uses reflection to find and create an instance of all classes marked with StateAttr.
		/// </summary>
		public static List<State> FindAll()
		{
			var states = new List<State>();

			var types = from type in Assembly.GetExecutingAssembly().GetTypes()
						where type.IsDefined(typeof(StateAttr)) && type.IsSubclassOf(typeof(State))
			            select type;

			foreach(var type in types)
				states.Add(Activator.CreateInstance(type) as State);

			return states;
		}
	}
}