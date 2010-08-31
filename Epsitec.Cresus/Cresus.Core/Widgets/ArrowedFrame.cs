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
	public sealed class ArrowedFrame : Tile
	{
		public ArrowedFrame()
		{
			this.ArrowDirection = Direction.Down;
		}


		public override TileArrowMode ArrowMode
		{
			get
			{
				return Widgets.TileArrowMode.VisibleDirect;
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
				arrow.SetSurfaceColors (ArrowedFrame.SurfaceColors);
				arrow.MouseHilite = false;

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
