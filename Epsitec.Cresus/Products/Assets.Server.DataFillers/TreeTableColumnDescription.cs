//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.DataFillers
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
	}
}