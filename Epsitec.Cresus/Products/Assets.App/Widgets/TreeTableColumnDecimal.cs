//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Colonne de TreeTable affichant des montants alignés à droite.
	/// </summary>
	public class TreeTableColumnDecimal : AbstractTreeTableColumn
	{
		public void SetCellDecimals(TreeTableCellDecimal[] cellDecimals)
		{
			this.cellDecimals = cellDecimals;
			this.Invalidate ();
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation(graphics, clipRect);

			if (this.cellDecimals != null)
			{
				int y = 0;

				foreach (var cellDecimal in this.cellDecimals)
				{
					var rect = this.GetCellsRect (y);

					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (AbstractTreeTableColumn.GetCellColor(y == this.hilitedHoverRow, cellDecimal.IsSelected));

					if (cellDecimal.Value.HasValue)
					{
						rect.Deflate (0, this.DescriptionMargin, 0, 0);

						var font = Font.DefaultFont;
						var text = cellDecimal.Value.Value.ToString ("0,0.00");
						//?var text = cellDecimal.Value.Value.ToString ("D2");

						graphics.Color = ColorManager.TextColor;
						graphics.PaintText (rect, text, font, this.FontSize, ContentAlignment.MiddleRight);
					}

					y++;
				}
			}
		}


		private TreeTableCellDecimal[] cellDecimals;
	}
}
