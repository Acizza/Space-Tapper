using System;
using SFML.Graphics;
using SFML.Window;
using System.Timers;

namespace SpaceTapper
{
	public class Game
	{
		public static Game Instance { get; private set; }
		public RenderWindow Window { get; private set; }
		public GameSettings Settings { get; private set; }
		public GameTime Time { get; private set; }
		public DateTime StartTime { get; private set; }
		public Timer GameTimer { get; private set; }
		public Font GameFont { get; private set; }
		public Text TimeText { get; private set; }
		public Text ScoreText { get; private set; }
		public Player Player { get; private set; }
		public BlockSpawner BlockSpawner { get; private set; }

		public Game(GameSettings settings)
		{
			Instance = this;
			Settings = settings;

			if(Environment.OSVersion.Platform == PlatformID.Unix)
				LinuxUtil.XInitThreads();

			InitWindow(settings);
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

		private void CreateText()
		{
			GameFont  = new Font("data/fonts/DejaVuSans.ttf");
			TimeText  = new Text("Time:\t00:00", GameFont, 20);
			ScoreText = new Text("Score:\t0", GameFont, 20);

			TimeText.Position  = new Vector2f(5, 5);
			ScoreText.Position = new Vector2f(5, TimeText.GetGlobalBounds().Height + 10);
		}

		private void CreateEntities()
		{
			Player = new Player(new Vector2f(Window.Size.X / 2, Window.Size.Y / 2));
			BlockSpawner = new BlockSpawner(100);
		}

		public void StartGame()
		{
			CreateText();
			CreateEntities();

			GameTimer = new Timer(1000);
			GameTimer.Elapsed += (s, e) => UpdateGameTime();
			GameTimer.Start();

			StartTime = DateTime.Now;
		}

		public void EndGame()
		{
			Player.Alive = false;
			GameTimer.Stop();
		}

		public void Run()
		{
			Time = new GameTime(0.5f);
			Time.FpsUpdate += OnFpsUpdate;

			StartGame();

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

			var dt = (float)Time.DeltaTime.TotalSeconds;

			if(Player.Alive)
			{
				Player.Update(dt);
				BlockSpawner.Update(dt);

				if(BlockSpawner.CheckCollision(Player.GetGlobalBounds()))
					EndGame();
			}
		}

		private void Render()
		{
			Window.Clear();

			Window.Draw(Player);
			Window.Draw(BlockSpawner);

			Window.Draw(TimeText);
			Window.Draw(ScoreText);

			Window.Display();
		}

		private void UpdateGameTime()
		{
			var totalTime = DateTime.Now - StartTime;
			TimeText.DisplayedString = string.Format("Time:\t{0:00}:{1:00}", totalTime.Minutes, totalTime.Seconds);
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