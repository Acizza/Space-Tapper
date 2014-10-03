using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Util;
using SpaceTapper.States;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace SpaceTapper
{
	public static class Game
	{
		public static RenderWindow Window;
		public static State DefaultState;

		public static List<State> States { get; private set; }
		public static double DeltaTime   { get; private set; }
		public static bool Initialized   { get; private set; }

		static DateTime mLastFrameTime;

		static Game()
		{
			States = new List<State>();
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

			States = State.FindAll();

			Initialized = true;
			Log.Info("Initialization complete");
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

			// No need to check if the state is valid, as the function will do it for us.
			if(DefaultState != null)
				SetActiveState(DefaultState.Name);

			while(Window.IsOpen())
			{
				Window.DispatchEvents();

				Update();
				Draw();
			}
		}

		/// <summary>
		/// Calculates the next delta time and does other frame update operations.
		/// </summary>
		static void Update()
		{
			DeltaTime = (DateTime.UtcNow - mLastFrameTime).TotalSeconds;
			mLastFrameTime = DateTime.UtcNow;

			foreach(var state in States)
			{
				if(!state.Updating)
					continue;

				state.Update(DeltaTime);
			}
		}

		/// <summary>
		/// Clears the screen and draws the next frame.
		/// </summary>
		static void Draw()
		{
			Window.Clear();

			foreach(var state in States)
			{
				if(!state.Drawing)
					continue;

				state.Draw(Window);
			}

			Window.Display();
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
			return States.Find(x => x.Name == name);
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
				Log.Error("State not found: ", name);
				return -1;
			}

			return index;
		}

		static void InitWindow(GameSettings settings)
		{
			var title = String.IsNullOrEmpty(settings.Title) ? "Window" : settings.Title;
			var style = settings.Fullscreen ? Styles.Fullscreen : Styles.Close;

			Window = new RenderWindow(settings.Mode, title, style);
			Window.Closed += OnWindowClosed;
			Window.KeyPressed += OnKeyPressed;
			Window.KeyReleased += OnKeyReleased;

			Window.SetKeyRepeatEnabled(settings.KeyRepeat);
			Window.SetVerticalSyncEnabled(settings.Vsync);
		}

		static void OnWindowClosed(object sender, EventArgs e)
		{
			Log.Info("Exiting");
			Window.Close();
		}

		static void OnKeyPressed(object sender, KeyEventArgs e)
		{
			Input.ProcessKey(e, true);
		}

		static void OnKeyReleased(object sender, KeyEventArgs e)
		{
			Input.ProcessKey(e, false);
		}

		/// <summary>
		/// Calls any required platform-specific functions.
		/// </summary>
		static void InitPlatform()
		{
			switch(Environment.OSVersion.Platform)
			{
				case PlatformID.Unix:
					LinuxInit.XInitThreads();
					break;
			}
		}
	}
}