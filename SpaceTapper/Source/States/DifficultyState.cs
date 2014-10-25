using System;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.States.Data;
using SpaceTapper.UI;

namespace SpaceTapper.States
{
	[StateAttr]
	public class DifficultyState : ForegroundState
	{
		Text 	   _titleText;
		ButtonList _buttons;

		public DifficultyState()
		{
			base.Name = "difficulty_select";

			Input.Keys[Keyboard.Key.Escape] = p =>
			{
				if(!p)
					return;

				State.SetActive("menu", "game", false, true);
			};

			PopulateDrawables();
		}

		void PopulateDrawables()
		{
			_titleText          = new Text("Difficulty", Game.Fonts["default"], 40);
			_titleText.Origin   = new Vector2f(_titleText.GetLocalBounds().Width / 2, 0);
			_titleText.Position = new Vector2f(Game.Size.X / 2, (int)(Game.Size.Y * 0.2f));

			_buttons = new ButtonList(this);

			var names = Enum.GetNames(typeof(GameDifficulty));
			var pos   = (Game.Size / 2) - new Vector2i(0, names.Length * 25);

			for(int i = 0; i < names.Length; ++i)
			{
				var b = new Button(this, names[i], Game.Fonts["default"],
					new Vector2f(pos.X, pos.Y + 25 * i));

				int idxCopy = i;

				b.Pressed += () =>
				{
					var gState = State.Get("game") as GameState;

					gState.StartGame((GameDifficulty)idxCopy);
					State.SetActive(gState);
				};

				_buttons.Add(b);
			}
		}

		public override void Update(float dt)
		{
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			base.Draw(target, states);

			target.Draw(_titleText);

			foreach(var button in _buttons)
				target.Draw(button);
		}
	}
}