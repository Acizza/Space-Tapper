using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SFML.Graphics;
using SpaceTapper.Physics;
using SpaceTapper.Entities;

namespace SpaceTapper.Scenes
{
	public abstract class Scene : IUpdatable, Drawable
	{
		public static Dictionary<string, Type> Types;

		public Game Game             { get; protected set; }
		public Collider Collider     { get; protected set; }
		public Input Input           { get; protected set; }
		public List<Entity> Entities { get; protected set; }

		public bool Active
		{
			get
			{
				return _active;
			}
			set
			{
				_active = value;

				if(_active)
					Enter();
				else
					Leave();
			}
		}

		bool _active;

		static Scene()
		{
			Types = new Dictionary<string, Type>();
			GetTypes();
		}

		protected Scene(Game game, bool active = false)
		{
			Input = new Input(game);
			Input.OnKeyProcess   += p => Active; // Only process input if the scene is active
			Input.OnMouseProcess += p => Active;

			Game     = game;
			Active   = active;
			Collider = new Collider();
		}

		public abstract void Enter();
		public abstract void Leave();

		public virtual void Update(GameTime time)
		{
			if(Entities != null)
				Collider.Update(Entities);
		}

		public abstract void Draw(RenderTarget target, RenderStates states);

		public static void GetTypes()
		{
			Types.Clear();

			var types = from type in Assembly.GetExecutingAssembly().GetTypes()
			            where type.IsDefined(typeof(GameSceneAttribute)) && type.IsSubclassOf(typeof(Scene))
			            select new { Attribute = type.GetCustomAttribute<GameSceneAttribute>(), Type = type };

			foreach(var type in types)
				Types[type.Attribute.Name] = type.Type;
		}
	}
}