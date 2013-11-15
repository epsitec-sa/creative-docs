//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public static class TreeTableColumnHelper
	{
		public static AbstractTreeTableColumn Create(TreeTableColumnDescription description)
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

				case TreeTableColumnType.Amount:
					column = new TreeTableColumnDecimal (DecimalFormat.Amount);
					break;

				case TreeTableColumnType.ComputedAmount:
					column = new TreeTableColumnComputedAmount (details: false);
					break;

				case TreeTableColumnType.DetailedComputedAmount:
					column = new TreeTableColumnComputedAmount (details: true);
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

				case TreeTableColumnType.Guid:
					column = new TreeTableColumnGuid ();
					break;
			}

			System.Diagnostics.Debug.Assert (column != null);

			column.HeaderDescription = description.Header;
			column.FooterDescription = description.Footer;

			return column;
		}
	}
}
