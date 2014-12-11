using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Scenes;
using SpaceTapper.UI;
using SpaceTapper.Util;

namespace SpaceTapper
{
	// Option file format is the same as parameters.
	[GameScene("options")]
	public class OptionsScene : Scene
	{
		struct Option
		{
			public Button Button;
			public TextField Field;

			public void Align()
			{
				Field.Origin   = new Vector2f(0, Button.GlobalBounds.Height);
				Field.Position = new Vector2f(Button.GlobalBounds.Right() + 25, Button.Position.Y);
			}

			public void UpdateText(string btnText, string fieldText = "")
			{
				Button.Text.DisplayedString = btnText.MakeFirstUpper() + ":";
				Field.Text.DisplayedString  = fieldText.MakeFirstUpper();
			}
		}

		class OptionList
		{
			public ButtonList Buttons;
			public List<TextField> Fields;

			public OptionList(Input input)
			{
				Buttons = new ButtonList(input);
				Fields  = new List<TextField>();
			}

			public void Add(Option option)
			{
				Buttons.Add(option.Button);
				Fields.Add(option.Field);
			}
		}

		public const string OptionsFile = "settings.cfg";

		OptionList _options;

		public OptionsScene(Game game) : base(game)
		{
			_options = new OptionList(Input);
			_options.Buttons.Scrolled += OnOptionsScrolled;

			var font = new Font("Resources/Fonts/DejaVuSans.ttf");
			var pos  = new Vector2f(Game.Window.Size.X / 2, Game.Window.Size.Y * 0.25f);

			foreach(var option in Options.All)
			{
				var pair = new Option
				{
					Button = new Button(this, font, pos, option.Key),
					Field  = new TextField(this, option.Value, font)
				};

				pair.Field.Enabled = false;
				pair.UpdateText(option.Key, option.Value);
				pair.Align();

				_options.Add(pair);
				pos.Y += 60;
			}

			Input.Keys.AddOrUpdate(Keyboard.Key.Escape, p => Game.SetActiveScene("menu"));
		}

		#region Private methods

		bool OnOptionsScrolled(int oldIdx, int newIdx, Button sButton, Keyboard.Key scrollKey)
		{
			// Skip any scrolling attempts if any fields are enabled so the user can type W and S
			if(_options.Fields.Any(x => x.Enabled))
			{
				if(scrollKey == Keyboard.Key.W || scrollKey == Keyboard.Key.S)
					return false;
			}

			_options.Fields[oldIdx].Enabled = false;
			_options.Fields[newIdx].Enabled = true;

			return true;
		}

		#endregion
		#region Public methods

		public void ReadOptions()
		{
			Options.ReadAll();
		}

		public override void Enter()
		{
			ReadOptions();
		}

		public override void Leave()
		{
			Options.WriteAll();
		}

		public override void Update(GameTime time)
		{

		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			foreach(var button in _options.Buttons.Buttons)
				target.Draw(button, states);

			foreach(var field in _options.Fields)
				target.Draw(field, states);
		}

		#endregion
	}
}