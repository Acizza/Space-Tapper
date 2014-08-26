using System;
using System.Linq;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using System.Diagnostics;
using System.IO;

namespace SpaceTapper
{
	/* Score format:
	 * score0:100
	 * score1:200
	 * score2:300
	*/

	public class ScoreboardState : AIdleState
	{
		public static int MaxScores = 5;
		public static readonly string ScoreFile = "scores.txt";

		Text mStateDesc;
		Text mLastScore;
		List<ScoreIter> mScores;

		public class ScoreIter
		{
			public int Score;
			public Text Text;

			public ScoreIter(int score, Text text)
			{
				Score = score;
				Text = text;
			}
		}

		public ScoreboardState(Game instance, bool active = false) : base(instance, active)
		{
			mStateDesc = new Text("Top Scores", GInstance.Fonts["default"], 40);
			mScores = new List<ScoreIter>();

			mStateDesc.Origin = mStateDesc.Size() / 2;
			mStateDesc.Position = new Vector2f(GInstance.Size.X / 2, 75);

			GInstance.GetState<GameState>().OnEndGame += HandleOnEndGame;

			if(!File.Exists(ScoreFile))
				File.WriteAllText(ScoreFile, "");

			ReadScores();

			base.OnKeyPressed += HandleOnKeyPressed;
		}

		~ScoreboardState()
		{
			WriteScores();
		}

		void WriteScores()
		{
			if(mScores.Count == 0)
				return;

			File.WriteAllText(ScoreFile, "");

			using(var sw = File.AppendText(ScoreFile))
			{
				for(int i = 0; i < mScores.Count; ++i)
					sw.WriteLine("score{0}:{1}", i, mScores[i].Score);
			}
		}

		public void ReadScores()
		{
			using(var sw = File.OpenText(ScoreFile))
			{
				string str;

				while((str = sw.ReadLine()) != null)
					AddScore(int.Parse(str.Split(':')[1]));
			}
		}

		/// <summary>
		/// Returns the score index if added, otherwise -1.
		/// </summary>
		/// <returns><c>true</c>, if score was added, <c>false</c> otherwise.</returns>
		/// <param name="score">Score.</param>
		public int AddScore(int score)
		{
			// Check for any scores that can be replaced.
			int index = mScores.FindIndex(x => x.Score <= score);

			if(index != -1 && mScores.Count >= MaxScores)
			{
				var s = mScores[index];

				s.Score = score;
				s.Text.DisplayedString = String.Format("{0}. {1}", index + 1, score);
				s.Text.Origin = s.Text.Size() / 2;

				return index;
			}

			// If we got to this point, the score was not high enough to replace anything.
			if(mScores.Count >= MaxScores)
				return -1;

			var text = new Text();

			text.Font = GInstance.Fonts["default"];
			text.CharacterSize = 30;

			mScores.Add(new ScoreIter(score, text));
			mScores.Sort((b, a) => a.Score.CompareTo(b.Score));

			// Layout all scores so they're in order.
			for(int i = 0; i < mScores.Count; ++i)
			{
				var s = mScores[i];

				s.Text.DisplayedString = String.Format("{0}. {1}", i + 1, s.Score);
				s.Text.Position = new Vector2f(GInstance.Size.X / 2, 300 + ((int)s.Text.Size().Y + 15) * i);
				s.Text.Origin = s.Text.Size() / 2;
			}

			return mScores.Count - 1;
		}

		public override void Update(TimeSpan dt)
		{
		}

		public override void Draw(RenderWindow window)
		{
			base.Draw(window);

			foreach(var i in mScores)
				window.Draw(i.Text);

			window.Draw(mStateDesc);
		}

		void HandleOnKeyPressed(KeyEventArgs e)
		{
			if(e.Code == Keyboard.Key.Escape)
				GInstance.OnEndFrame += OnEscapePressed;
		}

		void OnEscapePressed()
		{
			GInstance.SetActiveState(State.EndGame);
			GInstance.SetStateStatus(State.Game, false, true);

			GInstance.OnEndFrame -= OnEscapePressed;
		}

		void HandleOnEndGame()
		{
			// If the score was added, highlight it.
			int i = AddScore(GInstance.GetState<GameState>().Score);

			if(i == -1)
				return;

			if(mLastScore != null)
				mLastScore.Color = Color.White;

			var txt = mScores[i].Text;

			txt.Color = Color.Red;
			mLastScore = txt;
		}
	}
}