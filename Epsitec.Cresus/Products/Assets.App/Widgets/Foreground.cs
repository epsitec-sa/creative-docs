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
	/// Comme il vient par dessous les colonnes, il empêche leurs tooltips dynamiques
	/// de fonctionner. Il prend donc le relai et demande au TreeTable sous-jacent
	/// de trouver le bon tooltip.
	/// </summary>
	public class Foreground : FrameBox, Epsitec.Common.Widgets.Helpers.IToolTipHost
	{
		public Foreground()
		{
			this.zones = new List<Zone> ();

			ToolTip.Default.RegisterDynamicToolTipHost (this);  // pour voir les tooltips dynamiques
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
			var zone = new Zone (path, color, false);
			this.zones.Add (zone);
		}

		public void AddOutline(Path path, Color color, double width = 1, CapStyle cap = CapStyle.Round, JoinStyle join = JoinStyle.Round, double miterLimit = 10)
		{
			var zone = new Zone (path, color, true, width, cap, join, miterLimit);
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


		#region IToolTipHost Members
		public object GetToolTipCaption(Point pos)
		{
			if (this.Parent is TreeTable)
			{
				//	Demande au TreeTable sous-jacent de trouver le bon tooltip dynamique.
				var treeTable = this.Parent as TreeTable;

				pos = this.MapClientToScreen (pos);
				return treeTable.GetTooltip (pos);
			}

			return null;
		}
		#endregion


		private struct Zone
		{
			public Zone(Path path, Color color, bool isOutline, double width = 1, CapStyle cap = CapStyle.Round, JoinStyle join = JoinStyle.Round, double miterLimit = 10)
			{
				this.Path       = path;
				this.Color      = color;
				this.IsOutline  = isOutline;
				this.Width      = width;
				this.Cap        = cap;
				this.Join       = join;
				this.MiterLimit = miterLimit;
			}

			public readonly Path      Path;
			public readonly Color     Color;
			public readonly bool      IsOutline;
			public readonly double    Width;
			public readonly CapStyle  Cap;
			public readonly JoinStyle Join;
			public readonly double    MiterLimit;
		}


		private readonly List<Zone> zones;
	}
}