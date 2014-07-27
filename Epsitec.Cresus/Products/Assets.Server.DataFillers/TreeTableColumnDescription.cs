//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public struct TreeTableColumnDescription
	{
		public TreeTableColumnDescription(ObjectField field, TreeTableColumnType type = TreeTableColumnType.String, int width = 100, string header = null, string headerTooltip = null, string footer = null)
		{
			this.Field         = field;
			this.Type          = type;
			this.Width         = width;
			this.Header        = header;
			this.HeaderTooltip = headerTooltip;
			this.Footer        = footer;
		}


		public readonly ObjectField				Field;
		public readonly TreeTableColumnType		Type;
		public readonly int						Width;
		public readonly string					Header;
		public readonly string					HeaderTooltip;
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
