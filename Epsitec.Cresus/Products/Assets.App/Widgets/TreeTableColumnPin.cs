//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Colonne de TreeTable affichant des icônes.
	/// </summary>
	public class TreeTableColumnPin : AbstractTreeTableColumn
	{
		protected override void PaintCell(Graphics graphics, Rectangle rect, int y, AbstractTreeTableCell c)
		{
			//	Dessine la punaise. En mode "unpin", elle n'est dessinée
			//	que lorsque la souris survole la ligne.
			var cell = c as TreeTableCellInt;

			if (cell.Value.HasValue && (cell.Value.Value == 1 || y == this.hilitedHoverRow))
			{
				var textRect = rect;
				textRect.Offset (-1, 1);

				string icon = cell.Value.Value == 1 ? "TreeTable.Pin" : "TreeTable.Unpin";
				var text = Misc.GetRichTextImg (icon, verticalOffset: 0, iconSize: new Size (16, 16));
				this.PaintText (graphics, textRect, text);
			}
		}

		protected override ContentAlignment RowContentAlignment
		{
			get
			{
				return ContentAlignment.MiddleCenter;
			}
		}
	}
}
