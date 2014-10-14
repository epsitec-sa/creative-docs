//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class TreeTableCellInt : AbstractTreeTableCell
	{
		public TreeTableCellInt(int? value, CellState cellState, string tooltip = null)
			: base (cellState, tooltip)
		{
			this.Value = value;
		}


		public readonly int?					Value;

		
		public override string ToString()
		{
			return this.Value.ToString () + base.ToString ();
		}
	}
}
