//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public static class TreeTableColumnHelper
	{
		public static AbstractTreeTableColumn Create(DataAccessor accessor, TreeTableColumnDescription description)
		{
			AbstractTreeTableColumn column = null;

			switch (description.Type)
			{
				case TreeTableColumnType.Tree:
					column = new TreeTableColumnTree ();
					break;

				case TreeTableColumnType.String:
					column = new TreeTableColumnString ();
					break;

				case TreeTableColumnType.Decimal:
					column = new TreeTableColumnDecimal (DecimalFormat.Real);
					break;

				case TreeTableColumnType.Rate:
					column = new TreeTableColumnDecimal (DecimalFormat.Rate);
					break;

				case TreeTableColumnType.Years:
					column = new TreeTableColumnDecimal (DecimalFormat.Years);
					break;

				case TreeTableColumnType.Amount:
					column = new TreeTableColumnDecimal (DecimalFormat.Amount);
					break;

				case TreeTableColumnType.ComputedAmount:
					column = new TreeTableColumnComputedAmount (details: false);
					break;

				case TreeTableColumnType.AmortizedAmount:
					column = new TreeTableColumnAmortizedAmount (accessor, details: false);
					break;

				case TreeTableColumnType.DetailedComputedAmount:
					column = new TreeTableColumnComputedAmount (details: true);
					break;

				case TreeTableColumnType.DetailedAmortizedAmount:
					column = new TreeTableColumnAmortizedAmount (accessor, details: true);
					break;

				case TreeTableColumnType.Int:
					column = new TreeTableColumnInt ();
					break;

				case TreeTableColumnType.Date:
					column = new TreeTableColumnDate ();
					break;

				case TreeTableColumnType.Glyph:
					column = new TreeTableColumnGlyph ();
					break;

				case TreeTableColumnType.Icon:
					column = new TreeTableColumnIcon ();
					break;

				case TreeTableColumnType.Pin:
					column = new TreeTableColumnPin ();
					break;
			}

			System.Diagnostics.Debug.Assert (column != null);

			column.Field             = description.Field;
			column.HeaderDescription = description.Header;
			column.HeaderTooltip     = description.HeaderTooltip;
			column.FooterDescription = description.Footer;

			return column;
		}
	}
}
