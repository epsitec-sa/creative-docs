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
	public class TreeTableColumnIcon : AbstractTreeTableColumn
	{
		protected override void PaintCell(Graphics graphics, Rectangle rect, int y, AbstractTreeTableCell c)
		{
			var cell = c as TreeTableCellString;
			System.Diagnostics.Debug.Assert (cell != null);

			if (!string.IsNullOrEmpty (cell.Value))
			{
				var textRect = rect;
				textRect.Offset (-1, 1);

				var text = Misc.GetRichTextImg (cell.Value, verticalOffset: 0, iconSize: new Size (16, 16));
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
