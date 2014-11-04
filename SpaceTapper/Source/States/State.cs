using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using SFML.Graphics;
using SpaceTapper.Util;

namespace SpaceTapper.States
{
	public abstract class State : Transformable, Drawable
	{
		public string Name;
		public bool Updating  { get; private set; }
		public bool Drawing   { get; private set; }
		public Input Input    { get; private set; }

		public static uint MaxDrawOrder     { get; private set; }
		public static List<State> Instances { get; private set; }

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

		public bool Active
		{
			get
			{
				return Updating && Drawing;
			}
			private set
			{
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

		protected State()
		{
			Input = new Input();

			// Only process input if the state is updating.
			Input.OnKeyProcess   = k => Updating;
			Input.OnMouseProcess = m => Updating;
		}

		protected State(uint drawOrder) : this()
		{
			DrawOrder = drawOrder;
		}

		protected State(string name, bool active = false) : this()
		{
			Name   = name;
			Active = active;
		}

		protected State(string name, uint drawOrder, bool active = false)
			: this(name, active)
		{
			DrawOrder = drawOrder;
		}

		#endregion

		public abstract void Update(float dt);
		public abstract void Draw(RenderTarget target, RenderStates states);

		#region Status changed callbacks

		/// <summary>
		/// Called when Updating is set to true.
		/// </summary>
		public virtual void Enter()
		{
		}

		/// <summary>
		/// Called when Updating is set to false.
		/// </summary>
		public virtual void Leave()
		{
			// Reset all keys when leaving
			foreach(var key in Input.Keys)
				key.Value.Invoke(false);
		}

		#endregion
		#region State modifiers

		/// <summary>
		/// Makes the state found by name active. Disables all others.
		/// </summary>
		/// <param name="name">Name.</param>
		public static void SetActive(string name)
		{
			int index = FetchIndex(name, x => x.Name == name);

			if(index == -1)
				return;

			SetStatus(index, true, true);

			Instances.Where(x => x.Name != name).ToList().ForEach(DisableState);
		}

		/// <summary>
		/// Makes the state found by name active. Modifies the status of the state found from <c>other</c>.
		/// Example use case: SetActiveState("menu", "game", false, true)
		/// The example above will make the game state draw with the menu state.
		/// </summary>
		/// <param name="name">State name.</param>
		/// <param name="other">Other state name.</param>
		/// <param name="updating">If set to <c>true</c>, sets other state's updating value.</param>
		/// <param name="drawing">If set to <c>true</c>, sets the other state's drawing value.</param>
		public static void SetActive(string name, string other, bool updating, bool drawing)
		{
			int index    = FetchIndex(name, x => x.Name == name);
			int otherIdx = FetchIndex(name, x => x.Name == other);

			if(index == -1 || otherIdx == -1)
				return;

			SetStatus(index, true, true);
			SetStatus(otherIdx, updating, drawing);

			Instances.Where(x => x.Name != name && x.Name != other).ToList().ForEach(DisableState);
		}

		/// <summary>
		/// Makes the state found by name active. Modifies the status of the state found from <c>other</c>.
		/// Example use case: SetActiveState("menu", Get("game"), false, true)
		/// The example above will make the game state draw with the menu state.
		/// </summary>
		/// <param name="name">State name.</param>
		/// <param name="other">Other state.</param>
		/// <param name="updating">If set to <c>true</c>, sets other state's updating value.</param>
		/// <param name="drawing">If set to <c>true</c>, sets the other state's drawing value.</param>
		public static void SetActive(string name, State other, bool updating, bool drawing)
		{
			int index    = FetchIndex(name, x => x.Name == name);
			int otherIdx = FetchIndex(other.Name, x => x.Name == other.Name);

			if(index == -1 || otherIdx == -1)
				return;

			SetStatus(index, true, true);
			SetStatus(otherIdx, updating, drawing);

			Instances.Where(x => x.Name != name && x.Name != other.Name).ToList().ForEach(DisableState);
		}

		/// <summary>
		/// Makes the state found by reference active. Disables all others.
		/// </summary>
		/// <param name="state">State.</param>
		public static void SetActive(State state)
		{
			int index = FetchIndex(state.Name, x => x == state);

			if(index == -1)
				return;

			Instances[index].Active = true;
			Instances[index].Enter();

			Instances.Where(x => x != state).ToList().ForEach(DisableState);
		}

		/// <summary>
		/// Sets the status of the state found by name.
		/// </summary>
		/// <param name="name">State name.</param>
		/// <param name="updating">Forwarded to the found state's Updating variable.</param>
		/// <param name="drawing">Forwarded to the found state's Drawing variable.</param>
		public static void SetStatus(string name, bool updating, bool drawing)
		{
			int index = FetchIndex(name, x => x.Name == name);

			if(index == -1)
				return;

			SetStatus(index, updating, drawing);
		}

		/// <summary>
		/// Sets the status of the state found by reference.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="updating">Forwarded to the found state's Updating variable.</param>
		/// <param name="drawing">Forwarded to the found state's Drawing variable.</param>
		public static void SetStatus(State state, bool updating, bool drawing)
		{
			int index = FetchIndex(state.Name, x => x == state);

			if(index == -1)
				return;

			SetStatus(index, updating, drawing);
		}

		/// <summary>
		/// Sets the state found by index in Instances updating and drawing variables.
		/// Also calls Enter() or Leave().
		/// </summary>
		/// <param name="index">Instances index.</param>
		/// <param name="updating">If set to <c>true</c>, sets the state's updating variable.</param>
		/// <param name="drawing">If set to <c>true</c>, sets the state's drawing variable.</param>
		public static void SetStatus(int index, bool updating, bool drawing)
		{
			var s = Instances[index];

			s.Updating = updating;
			s.Drawing  = drawing;

			if(updating)
				s.Enter();
			else
				s.Leave();
		}

		/// <summary>
		/// Finds a state by name in State.Instances. Returns null if not found.
		/// </summary>
		/// <returns>The state found.</returns>
		/// <param name="name">State name.</param>
		public static State Get(string name)
		{
			var found = Instances.Find(x => x.Name == name);

			if(found == null)
				Log.Error("State not found: ", name);

			return found;
		}

		/// <summary>
		/// Tries to find a state by name in State.Instances. Returns null if not found.
		/// </summary>
		/// <returns>The state found.</returns>
		/// <param name="name">State name.</param>
		/// <typeparam name="T">The state type to return as.</typeparam>
		public static T TryGet<T>(string name) where T : State
		{
			return Get(name) as T;
		}

		/// <summary>
		/// Finds a state by name in State.Instances. Throws if unable to cast to T.
		/// </summary>
		/// <returns>The state found.</returns>
		/// <param name="name">State name.</param>
		/// <typeparam name="T">The state type to return as.</typeparam>
		public static T Get<T>(string name) where T : State
		{
			return (T)Get(name);
		}

		/// <summary>
		/// Util function to automatically log an invalid index.
		/// </summary>
		/// <returns>The state index.</returns>
		/// <param name="name">Name.</param>
		/// <param name="pred">Delegate.</param>
		static int FetchIndex(string name, Predicate<State> pred)
		{
			int index = Instances.FindIndex(pred);

			if(index == -1)
			{
				Log.Error("State.FetchIndex(): State not found: ", name);
				return -1;
			}

			return index;
		}

		/// <summary>
		/// Sets the state's active variable to false and calls Leave() on the state.
		/// </summary>
		/// <param name="state">The state.</param>
		static void DisableState(State state)
		{
			state.Active = false;
			state.Leave();
		}

		#endregion
		#region Static methods

		/// <summary>
		/// Uses reflection to find all classes marked with StateAttr and inherit from State.
		/// </summary>
		public static IEnumerable<Type> GetAllTypes()
		{
			var types = from type in Assembly.GetExecutingAssembly().GetTypes()
					where type.IsDefined(typeof(StateAttribute)) && type.IsSubclassOf(typeof(State))
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