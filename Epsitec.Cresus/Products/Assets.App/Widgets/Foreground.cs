//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ce widget permet de dessiner une zone rectangulaire servant de feedback
	/// visuel pour un hilite, par exemple lors du déplacement d'une colonne.
	/// </summary>
	public class Foreground : Widget
	{
		public Foreground()
		{
			this.zones = new List<Zone> ();
		}


		public void ClearZones()
		{
			this.zones.Clear ();
		}

		public void AddZone(Rectangle rect, Color color)
		{
			var zone = new Zone
			{
				Rectangle = rect,
				Color     = color,
			};

			this.zones.Add (zone);
		}


		protected override void OnPaintBackground(PaintEventArgs e)
		{
			foreach (var zone in zones)
			{
				e.Graphics.AddFilledRectangle (zone.Rectangle);
				e.Graphics.RenderSolid (zone.Color);
			}
		}


		private struct Zone
		{
			public Rectangle Rectangle;
			public Color     Color;
		}


		private readonly List<Zone> zones;
	}
}