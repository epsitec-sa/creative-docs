//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// The <c>ArrowedTile</c> class is a container which paints a frame with an arrow on
	/// one of its sides. This is used as a container for the <see cref="Epsitec.Cresus.Core.Controllers.ListController&lt;T&gt;"/>.
	/// </summary>
	public class ArrowedTile : Tile
	{
		public ArrowedTile(Direction arrowDirection)
			: base (arrowDirection)
		{
			this.arrowMode = TileArrowMode.Selected;
			
			this.tileArrow.SetOutlineColors (TileColors.BorderColors);
			this.tileArrow.MouseHilite = false;
		}


		public override TileArrowMode ArrowMode
		{
			get
			{
				return this.GetPaintingArrowMode ();
			}
			set
			{
				throw new System.InvalidOperationException ("ArrowedTile.ArrowMode is read-only");
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if (string.IsNullOrWhiteSpace (this.Text))
			{
				return;
			}

			Rectangle rect = this.Client.Bounds;

			switch (this.Arrow.ArrowDirection)
			{
				case Direction.Right:
					rect = Rectangle.Deflate (rect, new Margins (0, TileArrow.Breadth, 0, 0));
					break;

				case Direction.Down:
					rect = Rectangle.Deflate (rect, new Margins (0, 0, 0, TileArrow.Breadth));
					break;
			}

			graphics.Color = Color.FromName ("Black");
			graphics.PaintText (rect, this.Text, Font.DefaultFont, Font.DefaultFontSize);
		}

		protected virtual TileArrowMode GetPaintingArrowMode()
		{
			return TileArrowMode.Selected;
		}
		
		protected override void UpdateTileArrow()
		{
			this.tileArrow.SetSurfaceColors (this.GetInternalSurfaceColors ());
		}

		private IEnumerable<Color> GetInternalSurfaceColors()
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
}
