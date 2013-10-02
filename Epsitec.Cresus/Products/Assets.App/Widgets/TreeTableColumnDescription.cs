//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TreeTableColumnDescription
	{
		public TreeTableColumnDescription(TreeTableColumnType type = TreeTableColumnType.String, int width = 100, string header = null, string footer = null, bool dockToLeft = false)
		{
			this.Type       = type;
			this.Width      = width;
			this.Header     = header;
			this.Footer     = footer;
			this.DockToLeft = dockToLeft;
		}


		public readonly TreeTableColumnType		Type;
		public readonly int						Width;
		public readonly string					Header;
		public readonly string					Footer;
		public readonly bool					DockToLeft;

		
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
				case TreeTableColumnType.Glyph:
					column = new TreeTableColumnGlyph ();
					break;

				case TreeTableColumnType.String:
					column = new TreeTableColumnString ();
					break;

				case TreeTableColumnType.Decimal:
					column = new TreeTableColumnDecimal ();
					break;
			}

			System.Diagnostics.Debug.Assert (column != null);

			column.DockToLeft        = description.DockToLeft;
			column.PreferredWidth    = description.Width;
			column.HeaderDescription = description.Header;
			column.FooterDescription = description.Footer;

			return column;
		}
	}
}
