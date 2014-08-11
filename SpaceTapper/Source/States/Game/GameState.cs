using System;
using System.Timers;
using SFML.Graphics;
using SFML.Window;

namespace SpaceTapper
{
	// TODO: Add some events. Ex: EndGame()
	public class GameState : AState
	{
		public DateTime StartTime { get; private set; }
		public Timer GameTimer { get; private set; }
		public Font GameFont { get; private set; }
		public Text TimeText { get; private set; }
		public Text ScoreText { get; private set; }
		public uint Score { get; private set; }

		public Player Player { get; private set; }
		public BlockSpawner BlockSpawner { get; private set; }

		public event Action OnStartGame = delegate {};
		public event Action OnEndGame = delegate {};

		public GameState(Game instance, bool active = true) : base(instance, active)
		{
		}

		public void StartNewGame()
		{
			Active = true;

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
			Player.Alive = false;

			GameTimer.Stop();
			OnEndGame.Invoke();
		}

		public override void Update(TimeSpan delta)
		{
			if(!Updating)
				return;

			Player.Update(delta);
			BlockSpawner.Update(delta);

			if(BlockSpawner.CheckCollision(Player.GetGlobalBounds()))
				EndGame();
		}

		public override void Draw(RenderWindow window)
		{
			if(!Drawing)
				return;

			window.Draw(Player);
			window.Draw(BlockSpawner);

			window.Draw(TimeText);
			window.Draw(ScoreText);
		}

		void CreateText()
		{
			GameFont  = new Font("data/fonts/DejaVuSans.ttf");
			TimeText  = new Text("Time:\t00:00", GameFont, 20);
			ScoreText = new Text("Score:\t0", GameFont, 20);

			TimeText.Position  = new Vector2f(5, 5);
			ScoreText.Position = new Vector2f(5, TimeText.GetGlobalBounds().Height + 10);
		}

		void CreateEntities()
		{
			var size = GInstance.Window.Size;

			Player = new Player(GInstance, new Vector2f(size.X / 2, size.Y / 2));
			BlockSpawner = new BlockSpawner(GInstance, 100);

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
			if(e.Code == Keyboard.Key.Escape)
				GInstance.SetActiveState(State.Menu);
		}
	}
}