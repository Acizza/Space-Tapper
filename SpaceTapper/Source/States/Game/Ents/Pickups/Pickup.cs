using System;
using SFML.Window;
using SFML.Graphics;
using System.Collections.Generic;
using System.Threading;

namespace SpaceTapper
{
	public class Pickup : ARectEntity
	{
		public static List<AUpgrade> Upgrades;
		public static Text ActivateText { get; private set; }
		public static bool ActivateTextShowing;

		public static readonly Vector2f Size = new Vector2f(10, 10);

		static Random mRand;

		static Pickup()
		{
			mRand = new Random();
		}

		public Pickup(Game instance) : base(instance)
		{
			if(Upgrades == null)
			{
				Upgrades = new List<AUpgrade>()
				{
					new SlowDownUpgrade(GInstance),
					new SlowBlocksUpgrade(GInstance),
					new VelocityBoostUpgrade(GInstance)
				};
			}

			if(ActivateText == null)
			{
				ActivateText = new Text("", GInstance.Fonts["default"], 16);
				ActivateText.Position = new Vector2f(10, GInstance.Size.Y - ActivateText.GetLocalBounds().Height - 30);
				ActivateText.Color = Color.White;
			}

			Shape = new RectangleShape(Size);
			Shape.FillColor = Color.Yellow;
		}

		public Pickup(Game instance, Vector2f pos) : this(instance)
		{
			Shape.Position = pos;
		}

		public override void DrawSelf(RenderTarget target, RenderStates states)
		{
			target.Draw(Shape, states);

			if(ActivateTextShowing)
				target.Draw(ActivateText);
		}

		public void Invoke()
		{
			var upgrade = Upgrades[mRand.Next(0, Upgrades.Count)];
			upgrade.Invoke();

			ActivateText.DisplayedString = upgrade.Name + " Activated";
			ActivateTextShowing = true;

			new Timer(e => DisableUpgrade((AUpgrade)e), upgrade, upgrade.ActiveTime, TimeSpan.FromMilliseconds(-1));
		}

		void DisableUpgrade(AUpgrade upgrade)
		{
			ActivateTextShowing = false;
			upgrade.Disable();
		}
	}
}