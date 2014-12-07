using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Scenes;
using SpaceTapper.Util;

namespace SpaceTapper
{
	public sealed class Game
	{
		/// <summary>
		/// Random number generator.
		/// </summary>
		public static Random Random = new Random();

		/// <summary>
		/// The render window used by the game.
		/// </summary>
		/// <value>The window.</value>
		public RenderWindow Window { get; private set; }

		/// <summary>
		/// The settings passed to the game on initialization.
		/// </summary>
		/// <value>The settings.</value>
		public GameSettings Settings { get; private set; }

		/// <summary>
		/// The game time. Updated every frame.
		/// </summary>
		/// <value>The game time.</value>
		public GameTime Time { get; private set; }

		/// <summary>
		/// Debug information shown by the game.
		/// </summary>
		/// <value>The debug info.</value>
		public DebugInfo DebugInfo { get; private set; }

		/// <summary>
		/// All initialized game scenes.
		/// </summary>
		/// <value>The game scenes.</value>
		public Dictionary<string, Scene> Scenes { get; private set; }

		/// <summary>
		/// Called at the end of every frame.
		/// </summary>
		public event Action EndFrame = delegate {};

		public Game(GameSettings settings)
		{
			Log.Info("Initializing");
			Settings = settings;

			InitializePlatform();
			InitializeWindow();
		}

		#region Initializing methods

		static void InitializePlatform()
		{
			switch(Environment.OSVersion.Platform)
			{
				case PlatformID.Unix:
					NativeMethods.XInitThreads();
					break;
			}
		}

		void InitializeWindow()
		{
			Window = new RenderWindow(Settings.Mode, Settings.Title, Settings.Style, Settings.CSettings);
			Window.SetVerticalSyncEnabled(Settings.Vsync);
			Window.SetKeyRepeatEnabled(Settings.KeyRepeat);

			Window.Closed += (sender, e) => Exit();
		}

		void InitializeScenes()
		{
			if(String.IsNullOrEmpty(Settings.DefaultScene))
				throw new ArgumentNullException("Default scene must be specified in game settings.");

			Scenes = new Dictionary<string, Scene>();

			foreach(var type in Scene.Types)
				Scenes.Add(type.Key, (Scene)Activator.CreateInstance(type.Value, this));

			SetActiveScene(Settings.DefaultScene);
		}

		void InitializeResources()
		{
			Log.Info("Initializing resources");

			DebugInfo = new DebugInfo(TimeSpan.FromSeconds(0.25f),
				new Font("Resources/Fonts/DejaVuSansMono.ttf"), 14);

			DebugInfo.Position = new Vector2f(5, 5);

			Time = new GameTime();

			InitializeScenes();
		}

		#endregion
		#region Game loop

		public void Run()
		{
			InitializeResources();

			Log.Info("Finished initializing");

			while(Window.IsOpen())
			{
				Window.DispatchEvents();

				Update();
				Draw();

				EndFrame.Invoke();
			}
		}

		void Update()
		{
			Time.Update();
			DebugInfo.Update(Time);

			foreach(var scene in Scenes)
			{
				if(!scene.Value.Active)
					continue;

				scene.Value.Update(Time);
			}
		}

		void Draw()
		{
			Window.Clear();

			foreach(var scene in Scenes)
			{
				if(!scene.Value.Active)
					continue;

				Window.Draw(scene.Value);
			}

			Window.Draw(DebugInfo);
			Window.Display();
		}

		#endregion
		#region Scene status modifiers

		/// <summary>
		/// Sets the active scene.
		/// </summary>
		/// <param name="name">Scene name.</param>
		public void SetActiveScene(string name)
		{
			SetSceneStatus(name, true);
		}

		/// <summary>
		/// Sets the active state of the specified scene. Makes all other scenes inactive.
		/// </summary>
		/// <param name="name">Scene name.</param>
		/// <param name="active">The active state to set the scene to.</param>
		public void SetSceneStatus(string name, bool active)
		{
			if(!Scenes.ContainsKey(name))
			{
				Log.Error("No state named \"" + name + "\" found");
				return;
			}

			Scenes[name].Active = active;

			if(active)
				Scenes.Where(x => x.Key != name).ToList().ForEach(x => x.Value.Active = false);

			Log.Info("Status of \"" + name + "\" set to " + active);
		}

		/// <summary>
		/// Returns a scene found by name. Automatically casts to T.
		/// </summary>
		/// <returns>The scene found.</returns>
		/// <param name="name">Scene name to search for.</param>
		/// <typeparam name="T">The scene type to return as.</typeparam>
		public T GetScene<T>(string name) where T : Scene
		{
			if(!Scenes.ContainsKey(name))
				throw new KeyNotFoundException("No scene named \"" + name + "\" found.");

			return (T)Scenes[name];
		}

		#endregion

		/// <summary>
		/// Closes the window.
		/// </summary>
		public void Exit()
		{
			if(Window == null || !Window.IsOpen())
				return;

			Log.Info("Exiting");
			Window.Close();
		}
	}
}