//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.NodeGetters;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class TreeTableCellTree : AbstractTreeTableCell
	{
		public TreeTableCellTree(int level, NodeType type, string value, CellState cellState)
			: base (cellState)
		{
			this.Level = level;
			this.Type  = type;
			this.Value = value;
		}


		public readonly int						Level;
		public readonly NodeType				Type;
		public readonly string					Value;

		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			buffer.Append (this.Value);
			buffer.Append (" ");
			buffer.Append (this.Type);
			buffer.Append (" ");
			buffer.Append (this.Level);

			return buffer.ToString () + base.ToString ();
		}
	}
}
