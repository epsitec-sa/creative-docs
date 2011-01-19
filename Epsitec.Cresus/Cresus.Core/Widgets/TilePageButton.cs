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
	public sealed class TilePageButton : Tiles.Tile
	{
		public TilePageButton()
			: base (Direction.Down)
		{
		}

		public TilePageButton(TabPageDef tabPageDef)
			: this ()
		{
			this.tabPageDef    = tabPageDef;
			this.Name          = tabPageDef.Name;
			this.FormattedText = tabPageDef.Text;
		}


		public TabPageDef TabPageDef
		{
			get
			{
				return this.tabPageDef;
			}
		}

		public override Tiles.TileArrowMode ArrowMode
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

		public override Tiles.TileArrow TileArrow
		{
			get
			{
				this.tileArrow.SetOutlineColors (Tiles.Tile.BorderColors);
				this.tileArrow.SetSurfaceColors (this.SurfaceColors);
				this.tileArrow.MouseHilite = this.MouseHilite;

				return this.tileArrow;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			var rect = this.ContainerBounds;

			graphics.Color = Color.FromName ("Black");
			graphics.PaintText (rect.Left, rect.Bottom, rect.Width, rect.Height, this.Text, Font.DefaultFont, Font.DefaultFontSize, Common.Drawing.ContentAlignment.MiddleCenter);
		}

		private Tiles.TileArrowMode GetPaintingArrowMode()
		{
			if (this.IsSelected)
			{
				return Tiles.TileArrowMode.Selected;
			}

			return Tiles.TileArrowMode.Normal;
		}

		private bool MouseHilite
		{
			get
			{
				return Misc.ColorsCompare (this.SurfaceColors, Tiles.Tile.SurfaceHilitedColors);
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
						return Tiles.Tile.SurfaceHilitedSelectedColors;
					}
					else
					{
						return Tiles.Tile.SurfaceHilitedColors;
					}
				}
				else
				{
					if (this.IsSelected)
					{
						return Tiles.Tile.SurfaceSelectedContainerColors;
					}
					else
					{
						return Tiles.Tile.SurfaceSummaryColors;
					}
				}
			}
		}


		private readonly TabPageDef tabPageDef;
	}
}
