//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;

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


		protected override void PaintCell(Graphics graphics, Rectangle rect, int y, AbstractTreeTableCell c)
		{
			var cell = c as TreeTableCellDecimal;
			System.Diagnostics.Debug.Assert (cell != null);

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
					case DecimalFormat.Years:
						text = TypeConverters.DecimalToString (cell.Value);
						break;
				}

				this.PaintText (graphics, textRect, text);
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
	}
}
