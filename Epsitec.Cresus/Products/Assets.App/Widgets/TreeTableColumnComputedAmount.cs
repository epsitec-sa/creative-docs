﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.Server.Helpers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Colonne de TreeTable affichant des montants alignés à droite.
	/// </summary>
	public class TreeTableColumnComputedAmount : AbstractTreeTableColumn
	{
		public TreeTableColumnComputedAmount(bool details = false)
		{
			this.details = details;
		}


		public override void SetGenericCells(TreeTableColumnItem columnItem)
		{
			this.SetCells (columnItem.GetArray<TreeTableCellComputedAmount> ());
		}

		private void SetCells(TreeTableCellComputedAmount[] cells)
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

						string text;
						if (this.details)
						{
							text = TypeConverters.ComputedAmountToString (cell.Value);
						}
						else
						{
							text = TypeConverters.AmountToString (cell.Value.Value.FinalAmount);
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


		private readonly bool details;
		private TreeTableCellComputedAmount[] cells;
	}
}
