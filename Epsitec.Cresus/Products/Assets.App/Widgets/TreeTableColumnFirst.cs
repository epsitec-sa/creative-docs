//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class TreeTableColumnFirst : AbstractTreeTableColumn
	{
		public void SetCellFirsts(TreeTableCellFirst[] cellFirsts)
		{
			this.cellFirsts = cellFirsts;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation(graphics, clipRect);

			int y = 0;

			foreach (var cellFirst in this.cellFirsts)
			{
				var rect = this.GetCellsRect (y);
				rect.Deflate (this.DescriptionMargin * (cellFirst.Level+1), 0, 0, 0);

				var font = Font.DefaultFont;

				graphics.Color = ColorManager.TextColor;
				graphics.PaintText (rect, cellFirst.Description, font, this.FontSize, ContentAlignment.MiddleLeft);

				y++;
			}
		}


		private TreeTableCellFirst[] cellFirsts;
	}
}
