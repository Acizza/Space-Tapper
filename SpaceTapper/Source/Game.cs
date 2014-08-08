using System;
using SFML.Graphics;
using SFML.Window;
using System.Timers;

namespace SpaceTapper
{
	public class Game
	{
		public RenderWindow Window { get; private set; }
		public GameSettings Settings { get; private set; }
		public GameTime Time { get; private set; }

		public MenuState MenuState { get; private set; }
		public GameState GameState { get; private set; }

		public Game(GameSettings settings)
		{
			Settings = settings;

			if(Environment.OSVersion.Platform == PlatformID.Unix)
				LinuxUtil.XInitThreads();

			InitWindow(settings);

			MenuState = new MenuState(this, false); // TODO: Set to the active state after implementing portions of the menu.
			GameState = new GameState(this, true);
		}

		private void InitWindow(GameSettings settings)
		{
			Window = new RenderWindow(settings.Mode, settings.Title, settings.Style);

			Window.SetVerticalSyncEnabled(settings.Vsync);
			Window.SetKeyRepeatEnabled(false);

			Window.Closed += (s, e) => Window.Close();
			Window.KeyPressed += (s, e) =>
			{
				if(e.Code == Keyboard.Key.Escape)
					Window.Close();
			};
		}

		public void Run()
		{
			Time = new GameTime(0.5f);
			Time.FpsUpdate += OnFpsUpdate;

			GameState.StartNewGame();

			while(Window.IsOpen())
			{
				Window.DispatchEvents();

				Update();
				Render();
			}
		}

		private void Update()
		{
			Time.Update();

			MenuState.Update(Time.DeltaTime);
			GameState.Update(Time.DeltaTime);
		}

		private void Render()
		{
			Window.Clear();

			MenuState.Render(Window);
			GameState.Render(Window);

			Window.Display();
		}

		private void OnFpsUpdate(uint fps)
		{
			Window.SetTitle(Settings.Title + " | " + (fps / Time.FpsResetTime) + " fps");
		}
	}

	public struct GameSettings
	{
		public VideoMode Mode;
		public Styles Style;
		public string Title;
		public bool Vsync;
	}
}