using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using SpaceTapper.Physics;
using SpaceTapper.Scenes;
using SpaceTapper.Util;
using System.Linq;

namespace SpaceTapper
{
	public sealed class Game
	{
		public RenderWindow Window   { get; private set; }
		public GameSettings Settings { get; private set; }

		public GameTime Time       { get; private set; }
		public DebugInfo DebugInfo { get; private set; }

		public Dictionary<string, Scene> Scenes { get; private set; }

		public event Action OnEndFrame = delegate {};

		public Game(GameSettings settings)
		{
			Log.Info("Initializing");
			Settings = settings;

			InitializePlatform();
			InitializeWindow();
		}

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
			Scenes = new Dictionary<string, Scene>();

			foreach(var type in Scene.Types)
				Scenes.Add(type.Key, (Scene)Activator.CreateInstance(type.Value, this));

			SetActiveScene("game");
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

		public void Run()
		{
			InitializeResources();

			Log.Info("Finished initializing");

			while(Window.IsOpen())
			{
				Window.DispatchEvents();

				Update();
				Draw();

				OnEndFrame.Invoke();
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

		public void SetActiveScene(string name)
		{
			SetSceneStatus(name, true);
		}

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

		public void Exit()
		{
			if(Window == null || !Window.IsOpen())
				return;

			Log.Info("Exiting");
			Window.Close();
		}
	}
}