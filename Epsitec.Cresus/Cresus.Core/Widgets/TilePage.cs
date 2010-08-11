﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public sealed class TilePage : Tile
	{
		public TilePage()
		{
			this.ArrowDirection = Direction.Down;
		}

		public TilePage(TabPageDef tabPageDef)
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

				arrow.SetOutlineColors (Tile.BorderColors);
				arrow.SetThicknessColors (null);
				arrow.SetSurfaceColors (this.SurfaceColors);
				arrow.MouseHilite = this.MouseHilite;

				return arrow;
			}
		}

		public override TileArrow ReverseArrow
		{
			get
			{
				var arrow = new TileArrow ();

				arrow.SetOutlineColors (null);
				arrow.SetThicknessColors (null);
				arrow.SetSurfaceColors (null);
				arrow.MouseHilite = true;

				return arrow;
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

		private TileArrowMode GetPaintingArrowMode()
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


		private readonly TabPageDef tabPageDef;
	}
}
