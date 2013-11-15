//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Colonne de TreeTable affichant des chaînes alignées à gauche.
	/// </summary>
	public class TreeTableColumnGuid : AbstractTreeTableColumn
	{
		public override void SetCells(TreeTableColumnItem columnItem)
		{
			this.cells = columnItem.GetArray<TreeTableCellGuid> ();
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
					graphics.RenderSolid (this.GetCellColor (y == this.hilitedHoverRow, cell.IsSelected));

					//	Dessine le texte.
					if (!cell.Value.IsEmpty)
					{
						var textRect = this.GetContentDeflateRectangle (rect);
						this.PaintText (graphics, textRect, cell.Value.ToString ());  // TODO
					}

					//	Dessine la grille.
					this.PaintGrid (graphics, rect, y, this.hilitedHoverRow);

					y++;
				}
			}
		}


		private TreeTableCellGuid[] cells;
	}
}
