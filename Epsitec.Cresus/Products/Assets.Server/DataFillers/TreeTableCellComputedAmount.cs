//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class TreeTableCellComputedAmount : AbstractTreeTableCell
	{
		public TreeTableCellComputedAmount(ComputedAmount? value, CellState cellState)
			: base (cellState)
		{
			this.Value = value;
		}


		public readonly ComputedAmount?			Value;

		
		public override string ToString()
		{
			return this.Value.ToString () + base.ToString ();
		}
	}
}
