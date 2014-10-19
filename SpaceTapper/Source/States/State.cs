using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SFML.Graphics;

namespace SpaceTapper.States
{
	public abstract class State : Transformable, Drawable
	{
		public string Name;
		public Input  Input   { get; private set; }

		public static uint MaxDrawOrder     { get; private set; }
		public static List<State> Instances { get; private set; }

		protected bool _updating;
		protected bool _drawing;

		uint _drawOrder = MaxDrawOrder++;

		#region Getters and setters

		public uint DrawOrder
		{
			get
			{
				return _drawOrder;
			}
			set
			{
				ValidateDrawOrder(value);
				_drawOrder = value;
			}
		}

		public bool Updating
		{
			get
			{
				return _updating;
			}
			set
			{
				UpdateChanged(value);
				_updating = value;
			}
		}

		public bool Drawing
		{
			get
			{
				return _drawing;
			}
			set
			{
				DrawChanged(value);
				_drawing = value;
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

		#endregion
		#region Constructors

		static State()
		{
			Instances = new List<State>();

			foreach(var type in GetAllTypes())
				Instances.Add(Activator.CreateInstance(type) as State);
		}

		public State()
		{
			Input = new Input();

			// Only process input if the state is updating.
			Input.OnKeyProcess   = k => Updating;
			Input.OnMouseProcess = m => Updating;
		}

		public State(uint drawOrder) : this()
		{
			DrawOrder = drawOrder;
		}

		public State(string name, bool active = false) : this()
		{
			Name   = name;
			Active = active;
		}

		public State(string name, uint drawOrder, bool active = false)
			: this(name, active)
		{
			DrawOrder = drawOrder;
		}

		#endregion

		public abstract void Update(float dt);
		public abstract void Draw(RenderTarget target, RenderStates states);

		#region Status changed callbacks

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

		#endregion
		#region Static methods

		/// <summary>
		/// Uses reflection to find all classes marked with StateAttr and inherit from State.
		/// </summary>
		public static IEnumerable<Type> GetAllTypes()
		{
			var types = from type in Assembly.GetExecutingAssembly().GetTypes()
						where type.IsDefined(typeof(StateAttr)) && type.IsSubclassOf(typeof(State))
			            select type;

			return types;
		}

		/// <summary>
		/// Aligns the draw order for every state in Instances.
		/// Ex: setting a state's draw order to 1 will make every other state with a draw order of 1 or less above 1.
		/// </summary>
		/// <param name="drawOrder">Draw order to check against.</param>
		static void ValidateDrawOrder(uint drawOrder)
		{
			foreach(var state in Instances)
			{
				if(state.DrawOrder <= drawOrder)
					state._drawOrder += (drawOrder - state.DrawOrder) + 1;
			}
		}

		#endregion
	}
}