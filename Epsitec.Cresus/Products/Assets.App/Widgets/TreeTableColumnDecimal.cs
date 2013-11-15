//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.Helpers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Colonne de TreeTable affichant des montants alignés à droite.
	/// </summary>
	public class TreeTableColumnDecimal : AbstractTreeTableColumn
	{
		public TreeTableColumnDecimal(DecimalFormat format)
		{
			this.format = format;
		}


		public override void SetCells(TreeTableColumnItem columnItem)
		{
			this.cells = columnItem.GetArray<TreeTableCellDecimal> ();
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

						string text = null;

						switch (this.format)
						{
							case DecimalFormat.Rate:
								text = TypeConverters.RateToString (cell.Value);
								break;

							case DecimalFormat.Amount:
								text = TypeConverters.AmountToString (cell.Value);
								break;

							case DecimalFormat.Real:
								text = TypeConverters.DecimalToString (cell.Value);
								break;
						}

						this.PaintText (graphics, textRect, text);
					}

					//	Dessine la grille.
					this.PaintGrid (graphics, rect, y, this.hilitedHoverRow);

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


		private readonly DecimalFormat format;
		private TreeTableCellDecimal[] cells;
	}
}
