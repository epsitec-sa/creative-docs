//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget permet de dessiner un cadre avec une pointe/flèche sur l'un des côtés.
	/// Il sert de conteneur pour ListController.
	/// </summary>
	public sealed class ArrowedFrame : Tiles.Tile
	{
		public ArrowedFrame(Direction arrowDirection)
			: base (arrowDirection)
		{
		}


		public override Tiles.TileArrowMode ArrowMode
		{
			get
			{
				return Widgets.Tiles.TileArrowMode.Selected;
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
					return Tiles.Tile.SurfaceSelectedContainerColors;
				}
				else
				{
					return ArrowedFrame.SurfaceColors;
				}
			}
		}

		public static IEnumerable<Color> SurfaceColors
		{
			get
			{
				yield return Color.FromHexa ("f4f9ff");  // bleuté
			}
		}

		protected override int GroupedItemIndex
		{
			get
			{
				throw new System.NotImplementedException ();
			}
			set
			{
				throw new System.NotImplementedException ();
			}
		}

		protected override string GroupId
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}

	
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			var rect  = this.Client.Bounds;
			rect.Bottom += Tiles.TileArrow.Breadth;

			graphics.Color = Color.FromName ("Black");
			graphics.PaintText (rect.Left, rect.Bottom, rect.Width, rect.Height, this.Text, Font.DefaultFont, Font.DefaultFontSize, Common.Drawing.ContentAlignment.MiddleCenter);
		}
	}
}
