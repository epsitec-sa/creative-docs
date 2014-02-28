﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class TreeTableColumnAmortizedAmount : AbstractTreeTableColumn
	{
		public TreeTableColumnAmortizedAmount(bool details = false)
		{
			this.details = details;
		}


		protected override void PaintCell(Graphics graphics, Rectangle rect, int y, AbstractTreeTableCell c)
		{
			var cell = c as TreeTableCellAmortizedAmount;
			System.Diagnostics.Debug.Assert (cell != null);

			if (cell.Value.HasValue)
			{
				var textRect = this.GetContentDeflateRectangle (rect);

				string text;
				if (this.details)
				{
					//text = TypeConverters.ComputedAmountToString (cell.Value);  // TODO...
					text = TypeConverters.AmountToString (cell.Value.Value.FinalAmortizedAmount);
				}
				else
				{
					text = TypeConverters.AmountToString (cell.Value.Value.FinalAmortizedAmount);
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


		private readonly bool details;
	}
}