using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Ents;
using SpaceTapper.States.Data;
using SpaceTapper.Util;

namespace SpaceTapper.States
{
	[StateAttr]
	public class GameState : State
	{
		/// <summary>
		/// Gets a value indicating whether a game is in progress.
		/// </summary>
		/// <value><c>true</c> if in progress; otherwise, <c>false</c>.</value>
		public bool InProgress           { get; private set; }
		public Player Player             { get; private set; }
		public BlockSpawner BlockSpawner { get; private set; }
		public uint Score                { get; private set; }

		GameDifficulty _level;
		DifficultySettings _settings;

		/// <summary>
		/// The difficulty level. Can be swapped mid-game.
		/// </summary>
		public GameDifficulty Level
		{
			get
			{
				return _level;
			}
			set
			{
				_settings = Difficulty.Levels[value];
				_level    = value;

				BlockSpawner.Settings = _settings;
			}
		}

		public GameState() : base(0)
		{
			base.Name = "game";

			Player = new Player(this, Game.Size.ToFloat() / 2);
			Player.HitWall += () => EndGame();

			BlockSpawner = new BlockSpawner(this);

			Input.Keys[Keyboard.Key.Escape] = OnEscapePressed;
		}

		/// <summary>
		/// Starts a new game. Note: Does not switch states automatically.
		/// </summary>
		/// <param name="level">Difficulty level.</param>
		public void StartGame(GameDifficulty level)
		{
			Level = level;

			Player.Reset();
			BlockSpawner.Reset();

			InProgress = true;
		}

		/// <summary>
		/// Ends the game.
		/// </summary>
		/// <param name="transition">If set to <c>true</c>, the active state is set to end_game.</param>
		public void EndGame(bool transition = true)
		{
			InProgress = false;

			if(transition)
				Game.SetActiveState("end_game", this, false, true);
		}

		public override void UpdateChanged(bool flag)
		{
			if(!flag || InProgress)
				return;

			StartGame(Level);
		}

		void OnEscapePressed(bool pressed)
		{
			if(!pressed)
				return;

			Game.SetActiveState("menu", this, false, true);
		}

		public override void Update(float dt)
		{
			if(!InProgress)
				return;

			Player.Update(dt);
			BlockSpawner.Update(dt);

			foreach(var block in BlockSpawner.Blocks)
			{
				if(block.GetGlobalBounds().Intersects(
					Player.Shape.GlobalBounds(Player.Position - Player.Origin)))
				{
					EndGame();
					break;
				}
			}
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			target.Draw(Player);
			target.Draw(BlockSpawner);
		}
	}
}