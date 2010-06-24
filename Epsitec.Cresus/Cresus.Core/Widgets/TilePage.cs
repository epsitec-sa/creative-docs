//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget s'utilise un peu à la façon d'un TabPage, pour simuler des onglets
	/// avec une tuile ayant une flèche 'v' en bas.
	/// </summary>
	public class TilePage : Tile
	{
		public TilePage()
		{
			this.ArrowDirection = Direction.Down;
		}

		public TilePage(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}



		public override TileArrowMode ArrowMode
		{
			get
			{
				return this.GetPaintingArrowMode ();
			}
			set
			{
				throw new System.NotImplementedException ();
			}
		}

		public override TileArrow DirectArrow
		{
			get
			{
				var arrow = new TileArrow ();

				arrow.SetOutlineColors   (this.OutlineColors);
				arrow.SetThicknessColors (this.ThicknessColors);
				arrow.SetSurfaceColors   (this.SurfaceColors);
				arrow.MouseHilite = this.MouseHilite;

				return arrow;
			}
		}

		public override TileArrow ReverseArrow
		{
			get
			{
				var arrow = new TileArrow ();

				arrow.SetOutlineColors   (this.ReverseOutlineColors);
				arrow.SetThicknessColors (this.ReverseThicknessColors);
				arrow.SetSurfaceColors   (this.ReverseSurfaceColors);
				arrow.MouseHilite = true;

				return arrow;
			}
		}



		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose (disposing);
		}

		
		protected virtual TileArrowMode GetPaintingArrowMode()
		{
			if (this.IsSelected)
			{
				return Widgets.TileArrowMode.VisibleDirect;
			}

			return Widgets.TileArrowMode.None;
		}

		private bool MouseHilite
		{
			get
			{
				return Misc.ColorsCompare (this.SurfaceColors, Tile.SurfaceHilitedColors);
			}
		}

		private IEnumerable<Color> SurfaceColors
		{
			get
			{
				if (this.IsEntered)
				{
					if (this.IsSelected)
					{
						return Tile.SurfaceHilitedSelectedColors;
					}
					else
					{
						return Tile.SurfaceHilitedColors;
					}
				}
				else
				{
					if (this.IsSelected)
					{
						return Tile.SurfaceSelectedContainerColors;
					}
					else
					{
						return Tile.SurfaceSummaryColors;
					}
				}
			}
		}

		private IEnumerable<Color> OutlineColors
		{
			get
			{
				return Tile.BorderColors;
			}
		}

		private IEnumerable<Color> ThicknessColors
		{
			get
			{
				return null;
			}
		}

		private IEnumerable<Color> ReverseSurfaceColors
		{
			get
			{
				return null;
			}
		}

		private IEnumerable<Color> ReverseOutlineColors
		{
			get
			{
				return null;
			}
		}

		private IEnumerable<Color> ReverseThicknessColors
		{
			get
			{
				return null;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			var rect  = this.Client.Bounds;
			rect.Bottom += TileArrow.Breadth;

			graphics.Color = Color.FromName ("Black");
			graphics.PaintText (rect.Left, rect.Bottom, rect.Width, rect.Height, this.Text, Font.DefaultFont, Font.DefaultFontSize, Common.Drawing.ContentAlignment.MiddleCenter);
		}
	}
}
