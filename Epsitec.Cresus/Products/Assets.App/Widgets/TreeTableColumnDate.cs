//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.Helpers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Colonne de TreeTable affichant des montants alignés à droite.
	/// </summary>
	public class TreeTableColumnDate : AbstractTreeTableColumn
	{
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation(graphics, clipRect);

			int y = 0;

			foreach (var c in this.cells)
			{
				var cell = c as TreeTableCellDate;
				System.Diagnostics.Debug.Assert (cell != null);

				//	Dessine le fond.
				var rect = this.GetCellsRect (y);

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.GetCellColor (y == this.hilitedHoverRow, cell.IsSelected, cell.IsEvent));

				if (cell.IsUnavailable)
				{
					this.PaintUnavailable (graphics, rect, y, this.hilitedHoverRow);
				}

				//	Dessine le montant.
				if (cell.Value.HasValue)
				{
					var textRect = this.GetContentDeflateRectangle (rect);
					string text = TypeConverters.DateToString (cell.Value);

					this.PaintText (graphics, textRect, text);
				}

				//	Dessine la grille.
				this.PaintGrid (graphics, rect, y, this.hilitedHoverRow);

				y++;
			}
		}

		protected override ContentAlignment RowContentAlignment
		{
			get
			{
				return ContentAlignment.MiddleRight;
			}
		}
	}
}
