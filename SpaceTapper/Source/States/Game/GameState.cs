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
		public Text TimeText { get; private set; }
		public Text ScoreText { get; private set; }
		public Player Player { get; private set; }
		public BlockSpawner BlockSpawner { get; private set; }

		public event Action OnStartGame = delegate {};
		public event Action OnEndGame = delegate {};

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

		public uint Score
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
		uint mScore;

		public GameState(Game instance, bool active = true) : base(instance, active)
		{
			var size = GInstance.Size;
			var font = GInstance.Fonts["default"];

			TimeText  = new Text("Time:\t00:00", font, 20);
			ScoreText = new Text("Score:\t0", font, 20);

			TimeText.Position  = new Vector2f(5, 5);
			ScoreText.Position = new Vector2f(5, TimeText.GetGlobalBounds().Height + 10);

			Player = new Player(GInstance, new Vector2f(size.X / 2, size.Y / 2));
			BlockSpawner = new BlockSpawner(GInstance);

			Player.OnCollision += () => OnPlayerCollision();
		}

		public void StartNewGame(DifficultyLevel level)
		{
			Reset();

			BlockSpawner.Reset();
			BlockSpawner.Difficulty = Difficulty.Levels[level];

			Active = true;

			GameTimer = new Timer(1000);
			GameTimer.Elapsed += (s, e) => UpdateGameTime();
			GameTimer.Start();

			StartTime = DateTime.Now;
			OnStartGame.Invoke();
		}

		public void EndGame()
		{
			Active = false;

			GameTimer.Stop();
			OnEndGame.Invoke();
		}

		public override void Update(TimeSpan delta)
		{
			Player.Update(delta);
			BlockSpawner.Update(delta);

			foreach(var block in BlockSpawner.Blocks)
			{
				if(block.GetGlobalBounds().Intersects(Player.GlobalBounds))
				{
					EndGame();
					return;
				}

				if(!block.Scored && block.Position.Y >= Player.Shape.Position.Y)
				{
					++Score;
					block.Scored = true;
				}
			}
		}

		public override void Draw(RenderWindow window)
		{
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

		protected override void OnKeyPressed(KeyEventArgs e)
		{
			if(e.Code == Keyboard.Key.Escape)
				GInstance.OnEndFrame += ReturnToMenu;
		}

		public void Resume()
		{
			Updating = true;
			GameTimer.Start();
		}

		public void Pause()
		{
			GameTimer.Stop();
			Updating = false;
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