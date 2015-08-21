//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Colonne de TreeTable affichant des montants alignés à droite.
	/// </summary>
	public class TreeTableColumnAmortizedAmount : AbstractTreeTableColumn
	{
		public TreeTableColumnAmortizedAmount(DataAccessor accessor, bool details = false)
		{
			this.accessor = accessor;
			this.details = details;
		}


		protected override void PaintCell(Graphics graphics, Rectangle rect, int y, AbstractTreeTableCell c)
		{
			var cell = c as TreeTableCellAmortizedAmount;
			System.Diagnostics.Debug.Assert (cell != null);

			if (cell.Value.HasValue)
			{
				var textRect = this.GetContentDeflateRectangle (rect);

				decimal? value = null;

				if (this.Field == ObjectField.MainValueDelta)
				{
					//	ObjectField.MainValueDelta permet d'obtenir la même valeur que ObjectField.MainValue, mais
					//	on obtient la différence (-Amortization), plutôt que la valeur finale (FinalAmount).
					//	Amortization retourne une valeur positive lorsque la valeur finale a diminué. Il faut donc
					//	prendre -Amortization pour avoir la différence.
					if (cell.Value.Value.Amortization.HasValue)
					{
						value = -cell.Value.Value.Amortization.Value;
					}
				}
				else
				{
					value = cell.Value.Value.FinalAmount;
				}

				string text;

				if (this.details)
				{
					text = TypeConverters.AmountToString (value);
				}
				else
				{
					text = TypeConverters.AmountToString (value);
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


		private readonly DataAccessor			accessor;
		private readonly bool					details;
	}
}
