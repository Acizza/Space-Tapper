using System;
using System.Timers;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	public class GameState : AState
	{
		public DateTime StartTime { get; private set; }
		public Timer GameTimer { get; private set; }
		public RectangleShape BackgroundRect { get; private set; }
		public Text TimeText { get; private set; }
		public Text ScoreText { get; private set; }
		public Player Player { get; private set; }
		public BlockSpawner BlockSpawner { get; private set; }
		public bool InGame { get; private set; }

		public event Action OnStartGame = delegate {};
		public event Action OnEndGame = delegate {};
		public event Action OnResumeGame = delegate {};
		public event Action OnPauseGame = delegate {};

		public string Time
		{
			get
			{
				return mTimeStr;
			}
			set
			{
				mTimeStr = value;
				TimeText.DisplayedString = "Time:\t" + mTimeStr;
			}
		}

		public int Score
		{
			get
			{
				return mScore;
			}
			set
			{
				mScore = value;
				ScoreText.DisplayedString = "Score:\t" + mScore;
			}
		}

		string mTimeStr;
		int mScore;

		public GameState(Game instance, bool active = true) : base(instance, active)
		{
			BackgroundRect = new RectangleShape(GInstance.Size);
			BackgroundRect.FillColor = new Color(10, 10, 10);

			GameTimer = new Timer(1000);
			GameTimer.Elapsed += (s, e) => UpdateGameTime();

			InitText();

			Player = new Player(this);
			BlockSpawner = new BlockSpawner(this);

			Player.OnCollision += OnPlayerCollision;

			base.OnKeyPressed += KeyPressedHandler;
			base.OnStatusChanged += HandleOnStatusChanged;
		}

		public void StartNewGame(DifficultySettings settings)
		{
			Reset();
			BlockSpawner.Difficulty = settings;

			Active = true;
			InGame = true;

			GameTimer.Start();

			StartTime = DateTime.Now;
			OnStartGame.Invoke();
		}

		public void StartNewGame(DifficultyLevel level)
		{
			StartNewGame(Difficulty.Levels[level]);
		}

		public void EndGame()
		{
			Active = false;
			InGame = false;

			OnEndGame.Invoke();
		}

		public override void Update(TimeSpan delta)
		{
			Player.Update(delta);
			BlockSpawner.Update(delta);

			foreach(var block in BlockSpawner.Blocks)
			{
				if(block.GlobalBounds.Intersects(Player.GlobalBounds))
				{
					EndGame();
					return;
				}

				if(!block.Scored && block.Position.Y >= Player.Position.Y)
				{
					++Score;
					block.Scored = true;
				}
					
				foreach(Pickup pickup in block.Children)
				{
					if(pickup.GlobalBounds.Intersects(Player.GlobalBounds))
					{
						// TODO: Refactor.
						// This will prevent multiple upgrades from being active at the same time, but it's somewhat hacky.
						if(!Pickup.ActivateTextShowing)
							pickup.Invoke();

						block.Children.Remove(pickup);
						break;
					}
				}
			}
		}

		public override void Draw(RenderWindow window)
		{
			window.Draw(BackgroundRect);

			window.Draw(Player);
			window.Draw(BlockSpawner);

			window.Draw(TimeText);
			window.Draw(ScoreText);
		}

		void Reset()
		{
			Time = "00:00";
			Score = 0;

			Player.Reset();
			BlockSpawner.Reset();
		}

		void InitText()
		{
			var size = GInstance.Size;
			var font = GInstance.Fonts["default"];

			TimeText  = new Text("Time:\t00:00", font, 20);
			ScoreText = new Text("Score:\t0", font, 20);

			TimeText.Position  = new Vector2f(5, 5);
			ScoreText.Position = new Vector2f(5, TimeText.GetGlobalBounds().Height + 10);
		}

		void UpdateGameTime()
		{
			var totalTime = DateTime.Now - StartTime;
			Time = string.Format("{0:00}:{1:00}", totalTime.Minutes, totalTime.Seconds);
		}

		void OnPlayerCollision()
		{
			EndGame();
		}

		void KeyPressedHandler(KeyEventArgs e)
		{
			if(e.Code == Keyboard.Key.Escape)
				GInstance.OnEndFrame += ReturnToMenu;
		}

		public void Resume()
		{
			OnResumeGame.Invoke();
			GameTimer.Start();
		}

		public void Pause()
		{
			OnPauseGame.Invoke();
			GameTimer.Stop();
		}

		void HandleOnStatusChanged(bool updating, bool drawing)
		{
			if(updating && InGame)
				Resume();
			else if(!updating && InGame)
				Pause();
		}

		void ReturnToMenu()
		{
			Pause();

			GInstance.SetActiveState(State.Menu);
			Drawing = true;

			GInstance.OnEndFrame -= ReturnToMenu;
		}
	}
}