using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.States;
using SpaceTapper.UI;

namespace SpaceTapper
{
	[StateAttr]
	public class DifficultyState : State
	{
		Text mTitleText;
		ButtonList mButtons;

		public DifficultyState()
		{
			base.Name = "difficulty_select";

			mTitleText          = new Text("Difficulty", Game.Fonts["default"], 40);
			mTitleText.Origin   = new Vector2f(mTitleText.GetLocalBounds().Width / 2, 0);
			mTitleText.Position = new Vector2f(Game.Size.X / 2, (int)(Game.Size.Y * 0.2f));

			PopulateButtons();
		}

		void PopulateButtons()
		{
			mButtons = new ButtonList(this);

			var names = Enum.GetNames(typeof(GameDifficulty));
			var pos   = Game.Size / 2 - new Vector2i(0, names.Length * 25);

			for(int i = 0; i < names.Length; ++i)
			{
				var b = new Button(this, names[i], Game.Fonts["default"],
									new Vector2f(pos.X, pos.Y + 25 * i));

				var copy = i;
				b.Pressed += () => OnDifficultyPressed(copy);

				mButtons.Add(b);
			}
		}

		public override void Update(double dt)
		{

		}

		public override void Draw(RenderTarget target)
		{
			target.Draw(mTitleText);

			foreach(var button in mButtons)
				target.Draw(button);
		}

		static void OnDifficultyPressed(int index)
		{
			var state = Game.GetState("game") as GameState;
			var level = (GameDifficulty)index;

			if(state == null)
				return;

			state.StartGame(level);
			Game.SetActiveState(state);
		}
	}
}