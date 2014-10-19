using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.States;

namespace SpaceTapper
{
	public class DebugMenu : Transformable, Drawable
	{
		public Dictionary<State, DebugStateInfo> StateTimes;

		/// <summary>
		/// The font to use for all (new) states added.
		/// </summary>
		public Font Font = Game.Fonts["default"];
		public uint FontSize = 12;

		/// <summary>
		/// The spacing between entries.
		/// </summary>
		public int HeightOffset = 15;

		/// <summary>
		/// When true, tracks the timings of all added states and draws them.
		/// </summary>
		public bool Show;

		/// <summary>
		/// Gets the total height of all state entries.
		/// </summary>
		/// <value>The total height.</value>
		public float TotalHeight
		{
			get
			{
				return StateTimes.Values
					.Sum(x => x.Text.GetLocalBounds().Height + HeightOffset);
			}
		}

		public DebugMenu()
		{
			StateTimes = new Dictionary<State, DebugStateInfo>();
			Add(State.Instances.ToArray());
		}

		/// <summary>
		/// Adds all passed states' to StateTimes.
		/// </summary>
		/// <param name="states">States.</param>
		public void Add(params State[] states)
		{
			foreach(var state in states)
			{
				var text = new Text("", Font, FontSize);

				StateTimes[state] = new DebugStateInfo(text);
				UpdateText(state);

				text.Position = new Vector2f(0,
					(text.GetLocalBounds().Height + HeightOffset) * (StateTimes.Count - 1));
			}
		}

		/// <summary>
		/// Update all text objects.
		/// </summary>
		public void Update()
		{
			if(!Show)
				return;

			foreach(var state in State.Instances)
				UpdateText(state);
		}

		#region State updates

		// TODO: May want to remove all the StateTimes.ContainsKey checks, for performance.
		/// <summary>
		/// Call before updating a state.
		/// </summary>
		/// <param name="state">State.</param>
		public void PreUpdateState(State state)
		{
			if(!Show || !StateTimes.ContainsKey(state))
				return;

			StateTimes[state].UpdateStopwatch.Restart();
		}

		/// <summary>
		/// Call after updating a state.
		/// </summary>
		/// <param name="state">State.</param>
		public void PostUpdateState(State state)
		{
			if(!Show || !StateTimes.ContainsKey(state))
				return;

			StateTimes[state].UpdateStopwatch.Stop();
		}

		/// <summary>
		/// Call before drawing a state.
		/// </summary>
		/// <param name="state">State.</param>
		public void PreDrawState(State state)
		{
			if(!Show || !StateTimes.ContainsKey(state))
				return;

			StateTimes[state].DrawStopwatch.Restart();
		}

		/// <summary>
		/// Call after drawing a state.
		/// </summary>
		/// <param name="state">State.</param>
		public void PostDrawState(State state)
		{
			if(!Show || !StateTimes.ContainsKey(state))
				return;

			StateTimes[state].DrawStopwatch.Stop();
		}

		#endregion

		void UpdateText(State state)
		{
			var s = StateTimes[state];
			string statusFlags = "";

			if(state.Updating || state.Drawing)
			{
				statusFlags = "(";

				if(state.Updating)
					statusFlags += "u";
				if(state.Drawing)
					statusFlags += "d";

				statusFlags += ") ";
			}

			s.Text.DisplayedString = String.Format(
				"{0}:\n\tUpdate: {1:0.00} ms\n\tDraw: {2:0.00} ms",
				statusFlags + state.Name,
				s.UpdateStopwatch.Elapsed.TotalMilliseconds,
				s.DrawStopwatch.Elapsed.TotalMilliseconds);
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			if(!Show)
				return;

			states.Transform *= base.Transform;

			foreach(var state in StateTimes)
				target.Draw(state.Value.Text, states);
		}
	}

	public struct DebugStateInfo
	{
		public Text Text;
		public Stopwatch UpdateStopwatch;
		public Stopwatch DrawStopwatch;

		public DebugStateInfo(Text text)
		{
			Text = text;

			UpdateStopwatch = new Stopwatch();
			DrawStopwatch   = new Stopwatch();
		}
	}
}