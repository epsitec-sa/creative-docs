//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ce widget permet de dessiner des chemins quelconques servant de feedback
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

		public void AddSurface(Rectangle rect, Color color)
		{
			var path = new Path ();
			path.AppendRectangle (rect);

			this.AddSurface (path, color);
		}

		public void AddOutline(Rectangle rect, Color color, double width = 1, CapStyle cap = CapStyle.Round, JoinStyle join = JoinStyle.Round, double miterLimit = 10)
		{
			var path = new Path ();
			path.AppendRectangle (rect);

			this.AddOutline (path, color, width, cap, join, miterLimit);
		}

		public void AddSurface(Path path, Color color)
		{
			this.AddZone (path, color, false);
		}

		public void AddOutline(Path path, Color color, double width = 1, CapStyle cap = CapStyle.Round, JoinStyle join = JoinStyle.Round, double miterLimit = 10)
		{
			this.AddZone (path, color, true, width, cap, join, miterLimit);
		}

		private void AddZone(Path path, Color color, bool isOutline, double width = 1, CapStyle cap = CapStyle.Round, JoinStyle join = JoinStyle.Round, double miterLimit = 10)
		{
			var zone = new Zone
			{
				Path      = path,
				Color     = color,
				IsOutline = isOutline,
				Width     = width,
				Cap       = cap,
				Join      = join,
			};

			this.zones.Add (zone);
		}


		protected override void OnPaintBackground(PaintEventArgs e)
		{
			foreach (var zone in zones)
			{
				if (zone.IsOutline)
				{
					e.Graphics.Rasterizer.AddOutline (zone.Path, zone.Width, zone.Cap, zone.Join, zone.MiterLimit);
				}
				else
				{
					e.Graphics.Rasterizer.AddSurface (zone.Path);
				}

				e.Graphics.RenderSolid (zone.Color);
			}
		}


		private struct Zone
		{
			public Path      Path;
			public Color     Color;
			public bool      IsOutline;
			public double    Width;
			public CapStyle  Cap;
			public JoinStyle Join;
			public double    MiterLimit;
		}


		private readonly List<Zone> zones;
	}
}