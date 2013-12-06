//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Colonne de TreeTable affichant les glyphs des événements.
	/// </summary>
	public class TreeTableColumnGlyph : AbstractTreeTableColumn
	{
		public override void SetCells(TreeTableColumnItem columnItem)
		{
			this.cells = columnItem.GetArray<TreeTableCellGlyph> ();
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
					graphics.RenderSolid (this.GetCellColor (y == this.hilitedHoverRow, cell.IsSelected, cell.IsEvent));

					if (cell.IsUnavailable)
					{
						this.PaintUnavailable (graphics, rect, y, this.hilitedHoverRow);
					}

					//	Dessine le glyph.
					if (cell.Value.HasValue)
					{
						PaintEventGlyph.Paint (graphics, rect, cell.Value.Value);
					}

					//	Dessine la grille.
					this.PaintGrid (graphics, rect, y, this.hilitedHoverRow);

					y++;
				}
			}
		}


		private TreeTableCellGlyph[] cells;
	}
}
