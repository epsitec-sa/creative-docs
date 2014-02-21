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
	public class TreeTableColumnString : AbstractTreeTableColumn
	{
		protected override void PaintCell(Graphics graphics, Rectangle rect, int y, AbstractTreeTableCell c)
		{
			var cell = c as TreeTableCellString;
			System.Diagnostics.Debug.Assert (cell != null);

			if (!string.IsNullOrEmpty (cell.Value))
			{
				var textRect = this.GetContentDeflateRectangle (rect);
				this.PaintText (graphics, textRect, cell.Value);
			}
		}
	}
}
