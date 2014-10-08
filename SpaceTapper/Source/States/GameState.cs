using System;
using SFML.Graphics;
using SpaceTapper.States;

namespace SpaceTapper
{
	[StateAttr]
	public class GameState : State
	{
		/// <summary>
		/// The difficulty level. Can be swapped mid-game.
		/// </summary>
		public GameDifficulty Level
		{
			get
			{
				return mLevel;
			}
			set
			{
				// TODO: Update BlockSpawner values when implemented.
				mLevel = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether a game is in progress.
		/// </summary>
		/// <value><c>true</c> if in progress; otherwise, <c>false</c>.</value>
		public bool InProgress { get; private set; }

		GameDifficulty mLevel;

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

			// TODO
		}

		public override void UpdateChanged(bool flag)
		{
			if(!flag)
				return;

			StartGame(Level);
		}

		public override void Update(double dt)
		{

		}

		public override void Draw(RenderTarget target)
		{

		}
	}
}