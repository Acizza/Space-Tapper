using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.States;
using SpaceTapper.Util;

namespace SpaceTapper
{
	[StateAttribute]
	public class ScoresState : ForegroundState
	{
		public const string OldFilePrefix = ".old";

		public string ScoreFile = "scores.txt";
		public uint MaxScores   = 5;

		public Text TitleText { get; private set; }

		List<ScoreIter> _lastScores;
		List<ScoreIter> _scores;

		int _lastReplaceIndex;

		public IReadOnlyList<ScoreIter> Scores
		{
			get
			{
				return _scores;
			}
		}

		public ScoresState()
		{
			base.Name = "scores";

			TitleText = new Text("Scores", Game.Fonts["default"], 40);
			TitleText.Position = new Vector2f(
				Game.Size.X / 2 - TitleText.GetLocalBounds().Width / 2,
				Game.Size.Y * 0.225f);

			_scores = new List<ScoreIter>();

			Input.Keys[Keyboard.Key.Escape] = p =>
			{
				if(!p)
					return;

				State.SetActive("end_game", "game", false, true);
			};

			ReadScores();
			_lastScores = new List<ScoreIter>(Scores);
		}

		~ScoresState()
		{
			// Only write scores if something has changed.
			if(Scores.Except(_lastScores).Any())
				WriteScores();
		}

		/// <summary>
		/// Tries to add the score to Scores and sorts accordingly.
		/// </summary>
		/// <returns><c>true</c>, if score was added, <c>false</c> otherwise.</returns>
		/// <param name="score">The score to add.</param>
		public bool AddScore(uint score)
		{
			if(ReplaceScore(score))
				return true;

			if(Scores.Count >= MaxScores)
				return false;

			_scores.Add(new ScoreIter(score, new Text("", Game.Fonts["default"])));
			RebuildScoreOrder();

			return true;
		}

		/// <summary>
		/// Tries to replace a score of equal or lesser value with score.
		/// </summary>
		/// <returns><c>true</c>, if score was replaced, <c>false</c> otherwise.</returns>
		/// <param name="score">The score to use as a replacement.</param>
		public bool ReplaceScore(uint score)
		{
			int replaceIdx = _scores.FindIndex(x => x.Score <= score);

			if(replaceIdx == -1 || Scores.Count < MaxScores)
				return false;

			var s = Scores[replaceIdx];

			s.Score = score;
			s.Text.DisplayedString = String.Format("{0}. {1}", replaceIdx + 1, score);
			s.Text.Origin = new Vector2f(s.Text.GetLocalBounds().Width / 2, 0);
			s.Text.Color  = Color.Red;

			if(Scores.Count >= _lastReplaceIndex)
				Scores[_lastReplaceIndex].Text.Color = Color.White;

			_lastReplaceIndex = replaceIdx;
			return true;
		}

		/// <summary>
		/// Sorts Scores and updates their text accordingly.
		/// </summary>
		public void RebuildScoreOrder()
		{
			_scores.Sort((a, b) => b.Score.CompareTo(a.Score));

			for(int i = 0; i < Scores.Count; ++i)
			{
				var s = Scores[i];

				s.Text.DisplayedString = String.Format("{0}. {1}", i + 1, s.Score);
				s.Text.Origin   = new Vector2f(s.Text.GetLocalBounds().Width / 2, 0);

				s.Text.Position = new Vector2f(
					Game.Size.X / 2,
					Game.Size.Y * 0.35f + 35 * i).Round();
			}
		}

		/// <summary>
		/// Write all scores to ScoreFile.
		/// </summary>
		public void WriteScores()
		{
			if(Scores.Count == 0)
			{
				File.WriteAllText(ScoreFile, "");
				return;
			}

			if(File.Exists(ScoreFile + OldFilePrefix))
				File.Delete(ScoreFile + OldFilePrefix);

			File.Move(ScoreFile, ScoreFile + OldFilePrefix);
			File.WriteAllLines(ScoreFile, Scores.Select(x => x.Score).Select(x => x.ToString()));
		}

		/// <summary>
		/// Reads all scores from ScoreFile.
		/// </summary>
		public void ReadScores()
		{
			if(!File.Exists(ScoreFile))
				WriteScores();

			_lastScores = new List<ScoreIter>(Scores);
			_scores.Clear();

			var lines = File.ReadAllLines(ScoreFile);

			for(int i = 0; i < lines.Length; ++i)
			{
				var line  = lines[i];
				var score = uint.Parse(line);

				AddScore(score);
			}
		}

		public override void Update(float dt)
		{

		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			base.Draw(target, states);

			target.Draw(TitleText, states);

			foreach(var score in Scores)
				target.Draw(score.Text, states);
		}
	}

	/// <summary>
	/// Intended use is for ScoreState.
	/// </summary>
	public class ScoreIter
	{
		public uint Score;
		public Text Text;

		public ScoreIter(uint score, Text text)
		{
			Score = score;
			Text  = text;
		}
	}
}