using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.States;
using SpaceTapper.Util;

namespace SpaceTapper
{
	public static class Game
	{
		public static RenderWindow Window;
		public static State DefaultState;
		public static Resource<Font> Fonts;

		public static List<State> States { get; private set; }
		public static double DeltaTime   { get; private set; }
		public static bool Initialized   { get; private set; }

		public static event Action EndFrame = delegate {};

		static DateTime _lastFrameTime;

		/// <summary>
		/// Util function for the window size.
		/// </summary>
		/// <value>The window size, in int form.</value>
		public static Vector2i Size
		{
			get
			{
				return new Vector2i((int)Window.Size.X, (int)Window.Size.Y);
			}
			set
			{
				Window.Size = new Vector2u((uint)value.X, (uint)value.Y);
			}
		}

		/// <summary>
		/// Initializes the window and states.
		/// </summary>
		/// <param name="settings">Game settings.</param>
		public static void Init(GameSettings settings)
		{
			if(Initialized)
			{
				Log.Warning("Tried to call Game.Init() after already being initialized");
				return;
			}

			Log.Info("Initializing");

			InitPlatform();
			InitWindow(settings);
			InitResources();

			States = State.FindAll();

			Initialized = true;
			Log.Info("Initialization complete");
		}

		static void InitWindow(GameSettings settings)
		{
			var title = String.IsNullOrEmpty(settings.Title) ? "Window" : settings.Title;
			var style = settings.Fullscreen ? Styles.Fullscreen : Styles.Close;

			Window = new RenderWindow(settings.Mode, title, style);
			Window.Closed += (s, e) => Exit();

			Window.SetKeyRepeatEnabled(settings.KeyRepeat);
			Window.SetVerticalSyncEnabled(settings.Vsync);
		}

		/// <summary>
		/// Calls any required platform-specific functions.
		/// </summary>
		static void InitPlatform()
		{
			switch(Environment.OSVersion.Platform)
			{
				// Threads + SFML calls will likely cause a crash, and SFML doesn't call this for us.
				case PlatformID.Unix:
					LinuxInit.XInitThreads();
					break;
			}
		}

		/// <summary>
		/// Creates resource groups.
		/// </summary>
		static void InitResources()
		{
			Fonts = new Resource<Font>();
			Fonts["default"] = new Font("Resources/Fonts/DejaVuSans.ttf");
		}

		/// <summary>
		/// Starts updating and drawing the window. Runs in the current thread.
		/// </summary>
		public static void Run()
		{
			if(!Initialized)
			{
				Log.Warning("Tried to call Game.Run() without calling Game.Init() first");
				return;
			}

			if(DefaultState != null)
				SetActiveState(DefaultState.Name);
			else if(States != null && States.Count > 0)
			{
				Log.Info("No default state found. Using: " + States[0].Name);
				SetActiveState(States[0]);
			}
			else
			{
				Log.Error("Unable to find a valid game state in Game.Run()");
				return;
			}

			while(Window.IsOpen())
			{
				Window.DispatchEvents();

				Update();
				Draw();

				EndFrame.Invoke();
			}
		}

		/// <summary>
		/// Calculates the next delta time and does other frame update operations.
		/// </summary>
		static void Update()
		{
			DeltaTime = (DateTime.UtcNow - _lastFrameTime).TotalSeconds;
			_lastFrameTime = DateTime.UtcNow;

			foreach(var state in States)
			{
				if(!state.Updating)
					continue;

				state.Update((float)DeltaTime);
			}
		}

		/// <summary>
		/// Clears the screen and draws the next frame.
		/// </summary>
		static void Draw()
		{
			Window.Clear();

			States.Sort((a, b) => b.DrawOrder.CompareTo(a.DrawOrder));

			foreach(var state in States)
			{
				if(!state.Drawing)
					continue;

				state.Draw(Window);
			}

			Window.Display();
		}

		/// <summary>
		/// Exits the game by closing the window.
		/// </summary>
		public static void Exit()
		{
			if(!Initialized)
				return;

			Log.Info("Exiting");
			Window.Close();
		}

		/// <summary>
		/// Makes the state found by name active. Disables all others.
		/// </summary>
		/// <param name="name">Name.</param>
		public static void SetActiveState(string name)
		{
			int index = FetchStateIndex(name, x => x.Name == name);

			if(index == -1)
				return;

			States[index].Active = true;
			States.Where(x => x.Name != name).ToList().ForEach(x => x.Active = false);
		}

		/// <summary>
		/// Makes the state found by name active. Sets other's states to updating and drawing.
		/// Example use case: SetActiveState("menu", GetState("game"), false, true)
		/// The example above will make the game state draw behind the menu state.
		/// </summary>
		/// <param name="name">State name.</param>
		/// <param name="other">Other state name.</param>
		/// <param name="updating">If set to <c>true</c>, sets other state's updating value.</param>
		/// <param name="drawing">If set to <c>true</c>, sets the other state's drawing value.</param>
		public static void SetActiveState(string name, string other, bool updating, bool drawing)
		{
			int index    = FetchStateIndex(name, x => x.Name == name);
			int otherIdx = FetchStateIndex(name, x => x.Name == other);

			if(index == -1 || otherIdx == -1)
				return;

			States[index].Active = true;

			States[otherIdx].Updating = updating;
			States[otherIdx].Drawing  = drawing;

			States.Where(x => x.Name != name && x.Name != other).ToList().ForEach(x => x.Active = false);
		}

		/// <summary>
		/// Makes the state found by name active. Sets other's states to updating and drawing.
		/// Example use case: SetActiveState("menu", GetState("game"), false, true)
		/// The example above will make the game state draw behind the menu state.
		/// </summary>
		/// <param name="name">State name.</param>
		/// <param name="other">Other state.</param>
		/// <param name="updating">If set to <c>true</c>, sets other state's updating value.</param>
		/// <param name="drawing">If set to <c>true</c>, sets the other state's drawing value.</param>
		public static void SetActiveState(string name, State other, bool updating, bool drawing)
		{
			int index    = FetchStateIndex(name, x => x.Name == name);
			int otherIdx = FetchStateIndex(other.Name, x => x.Name == other.Name);

			if(index == -1 || otherIdx == -1)
				return;

			States[index].Active = true;

			States[otherIdx].Updating = updating;
			States[otherIdx].Drawing  = drawing;

			States.Where(x => x.Name != name && x.Name != other.Name).ToList().ForEach(x => x.Active = false);
		}

		/// <summary>
		/// Makes the state found by reference active. Disables all others.
		/// </summary>
		/// <param name="state">State.</param>
		public static void SetActiveState(State state)
		{
			int index = FetchStateIndex(state.Name, x => x == state);

			if(index == -1)
				return;

			States[index].Active = true;
			States.Where(x => x != state).ToList().ForEach(x => x.Active = false);
		}

		/// <summary>
		/// Sets the status of the state found by name.
		/// </summary>
		/// <param name="name">State name.</param>
		/// <param name="updating">Forwarded to the found state's Updating variable.</param>
		/// <param name="drawing">Forwarded to the found state's Drawing variable.</param>
		public static void SetStateStatus(string name, bool updating, bool drawing)
		{
			int index = FetchStateIndex(name, x => x.Name == name);

			if(index == -1)
				return;

			States[index].Updating = updating;
			States[index].Drawing  = drawing;
		}

		/// <summary>
		/// Sets the status of the state found by reference.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="updating">Forwarded to the found state's Updating variable.</param>
		/// <param name="drawing">Forwarded to the found state's Drawing variable.</param>
		public static void SetStateStatus(State state, bool updating, bool drawing)
		{
			int index = FetchStateIndex(state.Name, x => x == state);

			if(index == -1)
				return;

			States[index].Updating = updating;
			States[index].Drawing  = drawing;
		}

		/// <summary>
		/// Finds state by name in States. Returns null if not found.
		/// </summary>
		/// <returns>The state found.</returns>
		/// <param name="name">State name.</param>
		public static State GetState(string name)
		{
			var found = States.Find(x => x.Name == name);

			if(found == null)
				Log.Error("Game.GetState(): State not found: ", name);

			return found;
		}

		/// <summary>
		/// Util function to automatically log an invalid index.
		/// </summary>
		/// <returns>The state index.</returns>
		/// <param name="name">Name.</param>
		/// <param name="pred">Delegate.</param>
		static int FetchStateIndex(string name, Predicate<State> pred)
		{
			int index = States.FindIndex(pred);

			if(index == -1)
			{
				Log.Error("Game.FetchStateIndex(): State not found: ", name);
				return -1;
			}

			return index;
		}
	}
}