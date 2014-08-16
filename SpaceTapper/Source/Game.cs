using System;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using System.Timers;
using System.Collections.Generic;

namespace SpaceTapper
{
	public class Game
	{
		public RenderWindow Window;
		public GameSettings Settings { get; private set; }
		public GameTime Time { get; private set; }
		public Dictionary<State, AState> States;
		public Resource<Font> Fonts;

		public event Action OnEndFrame = delegate {};

		// Util function for floating point math.
		public Vector2f Size
		{
			get
			{
				return new Vector2f(Window.Size.X, Window.Size.Y);
			}
			set
			{
				Window.Size = new Vector2u((uint)value.X, (uint)value.Y);
			}
		}

		public Game(GameSettings settings)
		{
			Settings = settings;

			if(Environment.OSVersion.Platform == PlatformID.Unix)
				LinuxUtil.XInitThreads();

			InitWindow(settings);
			InitResources();

			Time = new GameTime(0.5f);
			Time.FpsUpdate += OnFpsUpdate;

			States = new Dictionary<State, AState>();

			States[State.Game] = new GameState(this, false);
			States[State.EndGame] = new EndGameState(this, false);
			States[State.Menu] = new MenuState(this, true);
			States[State.DifficultySelect] = new DifficultyState(this, false);
		}

		public void SetActiveState(State state)
		{
			States[state].Active = true;
			(from s in States where s.Key != state select s).ToList().ForEach(i => i.Value.Active = false);
		}

		public T GetState<T>(State state) where T : AState
		{
			return (T)States[state];
		}

		public void Run()
		{
			while(Window.IsOpen())
			{
				Window.DispatchEvents();

				Update();
				Render();

				OnEndFrame.Invoke();
			}
		}

		void Update()
		{
			Time.Update();

			foreach(var i in States)
			{
				if(!i.Value.Updating)
					continue;

				i.Value.Update(Time.DeltaTime);
			}
		}

		void Render()
		{
			Window.Clear();

			foreach(var i in States)
			{
				if(!i.Value.Drawing)
					continue;

				i.Value.Draw(Window);
			}

			Window.Display();
		}

		void InitResources()
		{
			Fonts = new Resource<Font>();
			Fonts["default"] = new Font("data/fonts/DejaVuSans.ttf");
		}

		void InitWindow(GameSettings settings)
		{
			Window = new RenderWindow(settings.Mode, settings.Title, settings.Style);

			Window.SetVerticalSyncEnabled(settings.Vsync);
			Window.SetKeyRepeatEnabled(false);

			Window.Closed += (s, e) => Window.Close();
		}

		void OnFpsUpdate(uint fps)
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

	public enum State
	{
		Menu,
		DifficultySelect,
		Game,
		EndGame
	}
}