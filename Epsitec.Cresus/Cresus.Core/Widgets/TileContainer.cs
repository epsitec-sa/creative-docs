//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>TileContainer</c> contains a stack of tiles. It paints a frame which
	/// takes into account the tile arrows.
	/// </summary>
	public class TileContainer : FrameBox
	{
		public TileContainer()
		{
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var adorner = Common.Widgets.Adorners.Factory.Active;
			var margins = new Margins (0.5, TileArrow.Breadth + 0.5, 0.5, 0.5);
			var frame   = Rectangle.Deflate (this.Client.Bounds, margins);

			graphics.AddRectangle (frame);
			graphics.RenderSolid (adorner.ColorBorder);
		}
	}
}
