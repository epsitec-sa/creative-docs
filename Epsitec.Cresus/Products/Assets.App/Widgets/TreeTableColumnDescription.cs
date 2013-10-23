//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TreeTableColumnDescription
	{
		public TreeTableColumnDescription(TreeTableColumnType type = TreeTableColumnType.String, int width = 100, string header = null, string footer = null)
		{
			this.Type   = type;
			this.Width  = width;
			this.Header = header;
			this.Footer = footer;
		}


		public readonly TreeTableColumnType		Type;
		public readonly int						Width;
		public readonly string					Header;
		public readonly string					Footer;

		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			buffer.Append (this.Type);
			buffer.Append (" ");
			buffer.Append (this.Width);
			buffer.Append (" ");
			buffer.Append (this.Header);
			buffer.Append (" ");
			buffer.Append (this.Footer);

			return buffer.ToString ();
		}


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
					column = new TreeTableColumnComputedAmount ();
					break;

				case TreeTableColumnType.Int:
					column = new TreeTableColumnInt ();
					break;

				case TreeTableColumnType.Date:
					column = new TreeTableColumnDate ();
					break;
			}

			System.Diagnostics.Debug.Assert (column != null);

			column.HeaderDescription = description.Header;
			column.FooterDescription = description.Footer;

			return column;
		}
	}
}
