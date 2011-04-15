//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// Ce widget permet de dessiner un cadre avec une pointe/flèche sur l'un des côtés.
	/// Il sert de conteneur pour ListController.
	/// </summary>
	public sealed class ArrowedTile : Tile
	{
		public ArrowedTile(Direction arrowDirection)
			: base (arrowDirection)
		{
		}


		public override TileArrowMode ArrowMode
		{
			get
			{
				return TileArrowMode.Selected;
			}
			set
			{
				throw new System.NotImplementedException ();
			}
		}

		public override TileArrow Arrow
		{
			get
			{
				this.tileArrow.SetOutlineColors (TileColors.BorderColors);
				this.tileArrow.SetSurfaceColors (this.InternalSurfaceColors);
				this.tileArrow.MouseHilite = false;

				return this.tileArrow;
			}
		}


		private IEnumerable<Color> InternalSurfaceColors
		{
			get
			{
				if (this.IsSelected)
				{
					return TileColors.SurfaceSelectedContainerColors;
				}
				else
				{
					return TileColors.SurfaceDefaultColors;
				}
			}
		}

	
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			var rect = Rectangle.Deflate (this.Client.Bounds, new Margins (0, 0, 0, TileArrow.Breadth));

			graphics.Color = Color.FromName ("Black");
			graphics.PaintText (rect.Left, rect.Bottom, rect.Width, rect.Height, this.Text, Font.DefaultFont, Font.DefaultFontSize, ContentAlignment.MiddleCenter);
		}
	}
}
