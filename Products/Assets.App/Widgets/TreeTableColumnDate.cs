//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Colonne de TreeTable affichant des montants alignés à droite.
	/// </summary>
	public class TreeTableColumnDate : AbstractTreeTableColumn
	{
		protected override void PaintCell(Graphics graphics, Rectangle rect, int y, AbstractTreeTableCell c)
		{
			if (c is TreeTableCellDate)
			{
				var cell = c as TreeTableCellDate;

				if (cell.Value.HasValue)
				{
					var textRect = this.GetContentDeflateRectangle (rect);
					string text = TypeConverters.DateToString (cell.Value);

					this.PaintText (graphics, textRect, text);
				}
			}

			//	Une colonne date peut parfois contenir un string. Lorsqu'il s'agit de représenter
			//	un intervalle de dates provenant de DateCumulValue, on affiche "...".
			if (c is TreeTableCellString)
			{
				var cell = c as TreeTableCellString;

				if (!string.IsNullOrEmpty (cell.Value))
				{
					var textRect = this.GetContentDeflateRectangle (rect);
					this.PaintText (graphics, textRect, cell.Value);
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
	}
}
