﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public void SetCells(TreeTableCellDecimal[] cells)
		{
			this.cells = cells;
			this.Invalidate ();
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation(graphics, clipRect);

			if (this.cells != null)
			{
				int y = 0;

				foreach (var cell in this.cells)
				{
					//	Dessine le fond.
					var rect = this.GetCellsRect (y);

					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (this.GetCellColor(y == this.hilitedHoverRow, cell.IsSelected));

					//	Dessine le montant.
					if (cell.Value.HasValue)
					{
						var textRect = this.GetContentDeflateRectangle (rect);

						var text = cell.Value.Value.ToString ("0,0.00");
						//?var text = cellDecimal.Value.Value.ToString ("D2");

						this.PaintText (graphics, textRect, text);
					}

					//	Dessine la grille.
					if (this.hilitedHoverRow != -1)
					{
						this.PaintGrid (graphics, rect, y, this.hilitedHoverRow);
					}

					y++;
				}
			}
		}

		protected override ContentAlignment RowContentAlignment
		{
			get
			{
				return ContentAlignment.MiddleRight;
			}
		}


		private TreeTableCellDecimal[] cells;
	}
}
