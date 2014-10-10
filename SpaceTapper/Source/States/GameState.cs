using System;
using SFML.Graphics;
using SpaceTapper.States.Data;
using SpaceTapper.Ents;
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
		public bool InProgress { get; private set; }
		public Player Player { get; private set; }

		GameDifficulty _level;

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
				// TODO: Update BlockSpawner values when implemented.
				_level = value;
			}
		}

		public GameState()
		{
			base.Name = "game";
		}

		/// <summary>
		/// Starts a new game. Note: Does not switch states automatically.
		/// </summary>
		/// <param name="level">Difficulty level.</param>
		public void StartGame(GameDifficulty level)
		{
			if(InProgress)
				return;

			if(Player == null)
			{
				Player = new Player(this, Game.Size.ToFloat() / 2);
				Player.HitWall += OnPlayerHitWall;
			}
			else
				Player.Reset();

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
				Game.SetActiveState("end_game");
		}

		public override void UpdateChanged(bool flag)
		{
			if(!flag)
				return;

			StartGame(Level);
		}

		public override void Update(float dt)
		{
			if(!InProgress)
				return;

			Player.Update(dt);
		}

		public override void Draw(RenderTarget target)
		{
			if(!InProgress)
				return;

			target.Draw(Player);
		}

		void OnPlayerHitWall()
		{
			// TODO: Check for horizontal wall allowance before instantly ending the game.
			EndGame();
		}
	}
}