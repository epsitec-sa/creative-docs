﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class TreeTableColumnString : AbstractTreeTableColumn
	{
		public void SetCellStrings(TreeTableCellString[] cellStrings)
		{
			this.cellStrings = cellStrings;
			this.Invalidate ();
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation(graphics, clipRect);

			int y = 0;

			foreach (var cellString in this.cellStrings)
			{
				var rect = this.GetCellsRect (y);

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (ColorManager.GetBackgroundColor ());

				rect.Deflate (this.DescriptionMargin, 0, 0, 0);

				var font = Font.DefaultFont;

				graphics.Color = ColorManager.TextColor;
				graphics.PaintText (rect, cellString.Value, font, this.FontSize, ContentAlignment.MiddleLeft);

				y++;
			}
		}


		private TreeTableCellString[] cellStrings;
	}
}