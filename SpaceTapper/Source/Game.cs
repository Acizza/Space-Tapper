using System;
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
				State.SetActive(DefaultState.Name);
			else if(State.Instances.Count > 0)
			{
				Log.Info("No default state found. Using: " + State.Instances[0].Name);
				State.SetActive(State.Instances[0]);
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

		static void OnDebugMenuKeyPressed(bool pressed)
		{
			if(!pressed)
				return;

			DebugMenu.Show = !DebugMenu.Show;
		}
	}
}