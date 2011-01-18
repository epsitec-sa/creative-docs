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
	/// Ce widget permet de dessiner un cadre avec une pointe/flèche sur l'un des côtés.
	/// Il sert de conteneur pour ListController.
	/// </summary>
	public sealed class ArrowedFrame : Tiles.Tile
	{
		public ArrowedFrame()
		{
			this.ArrowDirection = Direction.Down;
		}


		public override TileArrowMode ArrowMode
		{
			get
			{
				return Widgets.TileArrowMode.Selected;
			}
			set
			{
				throw new System.NotImplementedException ();
			}
		}

		public override TileArrow TileArrow
		{
			get
			{
				var arrow = new TileArrow ();

				arrow.SetOutlineColors (Tiles.Tile.BorderColors);
				arrow.SetThicknessColors (null);
				arrow.SetSurfaceColors (this.InternalSurfaceColors);
				arrow.MouseHilite = false;

				return arrow;
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
