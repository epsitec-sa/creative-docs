//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// Tuile simple (sans flèche) avec toujours un cadre et un fond neutre.
	/// </summary>
	public class FrameTile : Tile
	{
		public FrameTile()
			: base (Direction.Right)
		{
		}

		public FrameTile(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
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
			Rectangle rect = this.ContainerBounds;
			rect.Deflate (0.5);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Tile.SurfaceSummaryColors.First ());

			graphics.AddRectangle (rect);
			graphics.RenderSolid (Tile.BorderColors.First ());
		}
	}
}
