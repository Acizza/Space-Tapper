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
		public static Keyboard.Key DebugMenuKey = Keyboard.Key.Back;

		public static double DeltaTime    { get; private set; }
		public static bool Initialized    { get; private set; }
		public static Random Random       { get; private set; }
		public static Input Input         { get; private set; }
		public static DebugMenu DebugMenu { get; private set; }
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

		static Game()
		{
			Random = new Random();
		}

		#region Initialization

		/// <summary>
		/// Initializes the game.
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
		/// Creates resource groups and class instances.
		/// </summary>
		static void InitResources()
		{
			Fonts = new Resource<Font>();
			Fonts["default"] = new Font("Resources/Fonts/DejaVuSans.ttf");

			Input = new Input();
			Input.Keys[DebugMenuKey] = OnDebugMenuKeyPressed;

			//State.Instances = State.Instances;

			DebugMenu = new DebugMenu();
			DebugMenu.Position = new Vector2f(10, Window.Size.Y - DebugMenu.TotalHeight);
		}

		#endregion
		#region Game loop

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
			else if(State.Instances.Count > 0)
			{
				Log.Info("No default state found. Using: " + State.Instances[0].Name);
				SetActiveState(State.Instances[0]);
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

			DebugMenu.Update();

			foreach(var state in State.Instances)
			{
				if(!state.Updating)
					continue;

				DebugMenu.PreUpdateState(state);
				state.Update((float)DeltaTime);
				DebugMenu.PostUpdateState(state);
			}
		}

		/// <summary>
		/// Clears the screen and draws the next frame.
		/// </summary>
		static void Draw()
		{
			Window.Clear();

			State.Instances.Sort((a, b) => a.DrawOrder.CompareTo(b.DrawOrder));

			foreach(var state in State.Instances)
			{
				if(!state.Drawing)
					continue;

				DebugMenu.PreDrawState(state);
				Window.Draw(state);
				DebugMenu.PostDrawState(state);
			}

			Window.Draw(DebugMenu);
			Window.Display();
		}

		#endregion

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

		#region State modifiers

		/// <summary>
		/// Makes the state found by name active. Disables all others.
		/// </summary>
		/// <param name="name">Name.</param>
		public static void SetActiveState(string name)
		{
			int index = FetchStateIndex(name, x => x.Name == name);

			if(index == -1)
				return;

			State.Instances[index].Active = true;
			State.Instances.Where(x => x.Name != name).ToList().ForEach(x => x.Active = false);
		}

		/// <summary>
		/// Makes the state found by name active. Sets other's State.Instances to updating and drawing.
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

			State.Instances[index].Active = true;

			State.Instances[otherIdx].Updating = updating;
			State.Instances[otherIdx].Drawing  = drawing;

			State.Instances.Where(x => x.Name != name && x.Name != other).ToList().ForEach(x => x.Active = false);
		}

		/// <summary>
		/// Makes the state found by name active. Sets other's State.Instances to updating and drawing.
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

			State.Instances[index].Active = true;

			State.Instances[otherIdx].Updating = updating;
			State.Instances[otherIdx].Drawing  = drawing;

			State.Instances.Where(x => x.Name != name && x.Name != other.Name).ToList().ForEach(x => x.Active = false);
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

			State.Instances[index].Active = true;
			State.Instances.Where(x => x != state).ToList().ForEach(x => x.Active = false);
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

			State.Instances[index].Updating = updating;
			State.Instances[index].Drawing  = drawing;
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

			State.Instances[index].Updating = updating;
			State.Instances[index].Drawing  = drawing;
		}

		/// <summary>
		/// Finds state by name in State.Instances. Returns null if not found.
		/// </summary>
		/// <returns>The state found.</returns>
		/// <param name="name">State name.</param>
		public static State GetState(string name)
		{
			var found = State.Instances.Find(x => x.Name == name);

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
			int index = State.Instances.FindIndex(pred);

			if(index == -1)
			{
				Log.Error("Game.FetchStateIndex(): State not found: ", name);
				return -1;
			}

			return index;
		}

		#endregion

		static void OnDebugMenuKeyPressed(bool pressed)
		{
			if(!pressed)
				return;

			DebugMenu.Show = !DebugMenu.Show;
		}
	}
}