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
		public uint Score { get; private set; }
		public DifficultySettings CurDifficulty;

		public Player Player { get; private set; }
		public BlockSpawner BlockSpawner { get; private set; }

		public event Action OnStartGame = delegate {};
		public event Action OnEndGame = delegate {};

		public GameState(Game instance, bool active = true) : base(instance, active)
		{
		}

		public void StartNewGame(DifficultyLevel level)
		{
			GInstance.SetActiveState(State.Game);

			Score = 0;
			CurDifficulty = Difficulty.Levels[level];

			CreateText();
			CreateEntities();

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
					ScoreText.DisplayedString = "Score:\t" + ++Score;
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

		void CreateText()
		{
			var font = GInstance.Fonts["default"];

			TimeText  = new Text("Time:\t00:00", font, 20);
			ScoreText = new Text("Score:\t0", font, 20);

			TimeText.Position  = new Vector2f(5, 5);
			ScoreText.Position = new Vector2f(5, TimeText.GetGlobalBounds().Height + 10);
		}

		void CreateEntities()
		{
			var size = GInstance.Window.Size;

			Player = new Player(GInstance, new Vector2f(size.X / 2, size.Y / 2));
			BlockSpawner = new BlockSpawner(GInstance, CurDifficulty);

			Player.OnCollision += () => OnPlayerCollision();
		}

		void UpdateGameTime()
		{
			var totalTime = DateTime.Now - StartTime;
			TimeText.DisplayedString = string.Format("Time:\t{0:00}:{1:00}", totalTime.Minutes, totalTime.Seconds);
		}

		void OnPlayerCollision()
		{
			EndGame();
		}

		protected override void OnKeyPressed(KeyEventArgs e)
		{
			// If we don't set the state at the end of the frame,
			// the menu handler will pick the key press up too and exit the game.

			if(e.Code == Keyboard.Key.Escape)
				GInstance.OnEndFrame += EndFrameHandler;
		}

		void EndFrameHandler()
		{
			GInstance.SetActiveState(State.Menu);
			GInstance.OnEndFrame -= EndFrameHandler;
		}
	}
}